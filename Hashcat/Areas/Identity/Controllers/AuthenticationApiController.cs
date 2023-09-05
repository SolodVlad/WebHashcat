using Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebHashcat.Models;
using WebHashcat.Areas.Identity.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace WebHashcat.Areas.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _environment;
        private readonly TokenService _tokenService;

        public AuthenticationApiController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IConfiguration config, IEmailSender emailSender, IWebHostEnvironment environment, IDistributedCache cache)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _config = config;
            _emailSender = emailSender;
            _environment = environment;
            _tokenService = new TokenService(config, cache);
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

            //if (await _roleManager.FindByNameAsync("user") == null)
            //{
            //    var role = await _roleManager.CreateAsync(new IdentityRole("user"));
            //    if (role.Succeeded) await _userManager.AddToRoleAsync(user, "user");
            //}

            //if (await _roleManager.FindByNameAsync("admin") == null)
            //{
            //    var role = await _roleManager.CreateAsync(new IdentityRole("admin"));
            //    if (role.Succeeded) await _userManager.AddToRoleAsync(user, "admin");
            //}

            //if (await _roleManager.FindByNameAsync("manager") == null)
            //{
            //    var role = await _roleManager.CreateAsync(new IdentityRole("manager"));
            //    if (role.Succeeded) await _userManager.AddToRoleAsync(user, "manager");
            //}

            var res = await _userManager.CreateAsync(user, register.Password);
            if (!res.Succeeded) return new BadRequestObjectResult(res);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmLink = Url.Action("Confirm", "EmailConfirm", new { Area = "Identity", guid = token, userEmail = user.Email }, Request.Scheme, Request.Host.Value);

            await _emailSender.SendEmailAsync(user.Email, "Please activate link", $"<a href = {confirmLink}>Click to confirm email</a>");

            return StatusCode(201);

            //if (!res.Succeeded) return StatusCode(500, new Response() { Status = "Error", Message = "User create failed" });

            //return Ok(new Response() { Status = "Success", Message = "User created" });
        }

        //[HttpPost]
        //[Route("register-admin")]
        //public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        //{
        //    var userExists = await _userManager.FindByNameAsync(model.Username);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

        //    User user = new()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Username
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Value);
        //    if (!result.Succeeded)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        //    if (!await _roleManager.RoleExistsAsync(UserRoles.User))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        //    if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        //    }
        //    if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.User);
        //    }
        //    return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        //}

        [HttpPost]
        [Route("Login")]
        //[ValidateAntiForgeryToken]
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

                    await _tokenService.SaveRefreshTokenToCacheAsync(userNameHash, refreshToken);

                    AppendCookie("AuthCookie", jwtAccessToken, login.IsRememberMe);

                    var userBalance = user.Balance;

                    return Ok(userBalance);
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

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpGet]
        //[Route("Logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    Response.Cookies.Delete("AuthCookie");

        //    return Ok();
        //}

        [HttpPost]
        [Route("ValidateJWTToken")]
        public async Task<IActionResult> ValidateJwtToken([FromBody] string accessToken)
        {
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

                //var jwtSecurityToken = tokenHandler.ReadJwtToken(token.AccessToken);
                //var userName = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;

                var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

                var userName = principal.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
                var isRememberMe = Convert.ToBoolean(principal.Claims.First(claim => claim.Type == "isRememberMe").Value);

                var userNameHash = await ComputeSha512Async(Encoding.UTF8.GetBytes(userName));

                if (await _tokenService.IsRefreshTokenExistsAsync(userNameHash)) 
                {
                    var newJwtAccessSecurityToken = _tokenService.GenerateNewAccessToken(principal.Claims.ToList());
                    AppendCookie("AuthCookie", new JwtSecurityTokenHandler().WriteToken(newJwtAccessSecurityToken), isRememberMe);

                    var currentUser = await _userManager.FindByNameAsync(userName);

                    return Ok(new { userName, currentUser.Balance });
                }

                Response.Cookies.Delete("AuthCookie");
                return NoContent();
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("RevokeRefreshToken")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] string userName)
        {
            var userNameHash = await ComputeSha512Async(Encoding.UTF8.GetBytes(userName));
            if (!await _tokenService.IsRevokeRefreshTokenAsync(userNameHash)) return BadRequest("Invalid user name");

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("GetUserNameByAccessToken")]
        public string GetUserNameByAccessToken([FromBody] string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(accessToken);
            return jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
        }

        private void AppendCookie(string key, string value, bool isExpires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = !_environment.IsDevelopment(),
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