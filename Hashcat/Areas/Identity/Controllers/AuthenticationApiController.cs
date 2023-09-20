using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebHashcat.Areas.Identity.Services;
using Microsoft.Extensions.Caching.Distributed;
using WebHashcat.Areas.Identity.Models;

namespace WebHashcat.Areas.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;
        private readonly TokenService _tokenService;
        //private readonly UserBalanceManager _userBalanceManager;

        private readonly string _cookieName = "AuthCookie";

        public AuthenticationApiController(UserManager<User> userManager, IConfiguration config, IEmailSender emailSender, IDistributedCache cache/*, UserBalanceManager userBalanceManager*/)
        {
            _userManager = userManager;
            _config = config;
            _emailSender = emailSender;
            _tokenService = new TokenService(config, cache);
            //_userBalanceManager = userBalanceManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterAsync(Register register)
        {
            var user = await _userManager.FindByEmailAsync(register.Email);
            if (user != null) { return StatusCode(500, new Response() { Status = "Error", Message = "User already exists" }); }

            user = new User()
            {
                Email = register.Email,
                UserName = register.Login,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var res = await _userManager.CreateAsync(user, register.Password);
            if (!res.Succeeded) return new BadRequestObjectResult(res);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmLink = Url.Action("EmailConfirm", "Account", new { Area = "Identity", guid = token, userEmail = user.Email }, Request.Scheme, Request.Host.Value);

            await _emailSender.SendEmailAsync(user.Email, "Please activate link", $"<a href = {confirmLink}>Click to confirm email</a>");

            return StatusCode(201);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginAsync(Login login)
        {
            if (login != null)
            {
                var user = await _userManager.FindByNameAsync(login.Login_);
                if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("isRememberMe", login.IsRememberMe.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    foreach (var role in roles) authClaims.Add(new Claim(ClaimTypes.Role, role));

                    var jwtAccessToken = new JwtSecurityTokenHandler().WriteToken(_tokenService.GenerateNewAccessToken(authClaims));
                    var refreshToken = _tokenService.GenerateRefreshToken();

                    var userNameHash = await ComputeSha512Async(Encoding.UTF8.GetBytes(user.UserName));

                    if (!await _tokenService.IsRefreshTokenExistsAsync(userNameHash))
                        await _tokenService.SaveRefreshTokenToCacheAsync(userNameHash, refreshToken);

                    AppendCookie(_cookieName, jwtAccessToken, login.IsRememberMe);

                    return NoContent();
                }
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var link = Url.Action("ResetPassword", "Account", new { Area = "Identity", token, userEmail = user.Email }, Request.Scheme, Request.Host.Value);
            await _emailSender.SendEmailAsync(user.Email, "Reset password", $"<a href = {link}>Click to reset password</a>");
            return Ok();
        }

        [Route("ValidateJWTToken")]
        public async Task<IActionResult> ValidateJwtTokenAsync()
        {
            var accessToken = Request.Cookies[_cookieName];

            if (string.IsNullOrEmpty(accessToken)) return Ok();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("JWTSecret"));
            try
            {
                tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtSecurityToken = (JwtSecurityToken)validatedToken;
                var userName = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;

                var currentUser = await _userManager.FindByNameAsync(userName);

                return Ok(new { userName, currentUser.Balance });
            }
            catch (SecurityTokenExpiredException ex)
            {
                Debug.WriteLine(ex.Message);

                var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

                var userName = principal.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
                var isRememberMe = Convert.ToBoolean(principal.Claims.First(claim => claim.Type == "isRememberMe").Value);

                var userNameHash = await ComputeSha512Async(Encoding.UTF8.GetBytes(userName));

                if (await _tokenService.IsRefreshTokenExistsAsync(userNameHash)) 
                {
                    var newJwtAccessSecurityToken = _tokenService.GenerateNewAccessToken(principal.Claims.ToList());
                    AppendCookie(_cookieName, new JwtSecurityTokenHandler().WriteToken(newJwtAccessSecurityToken), isRememberMe);

                    var currentUser = await _userManager.FindByNameAsync(userName);

                    return Ok(new { userName, currentUser.Balance });
                }

                Response.Cookies.Delete(_cookieName);
                return Ok("Cookie deleted");
            }
            catch (Exception ex) 
            {
                Response.Cookies.Delete(_cookieName);
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("RevokeTokens")]
        public async Task<IActionResult> RevokeTokens([FromBody] string userName)
        {
            var userNameHash = await ComputeSha512Async(Encoding.UTF8.GetBytes(userName));
            if (!await _tokenService.IsRevokeRefreshTokenSuccessAsync(userNameHash)) return BadRequest("Invalid user name");
            
            Response.Cookies.Delete(_cookieName);
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("Logout")]
        public string GetUserNameByAccessToken()
        {
            var accessToken = Request.Cookies[_cookieName];
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(accessToken);
            
            return jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
        }

        private void AppendCookie(string key, string value, bool isExpires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                IsEssential = true
            };

            if (isExpires) cookieOptions.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append(key, value, cookieOptions);
        }

        private static async Task<string> ComputeSha512Async(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var hashBytes = await SHA512.Create().ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}