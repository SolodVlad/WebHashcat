using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebHashcatAdminPanel.Areas.Identity.Services;

namespace WebHashcatAdminPanel.Areas.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationApiController : ControllerBase
    {
        private readonly string _adminSha512 = "c7ad44cbad762a5da0a452f9e854fdc1e0e7a52a38015f23f3eab1d80b931dd472634dfac71cd34ebc35d16ab7fb8a90c81f975113d6c7538dc69dd8de9077ec";

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;
        private readonly TokenService _tokenService;

        private readonly string _cookieName = "AuthCookie";

        public AuthenticationApiController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IConfiguration config, IEmailSender emailSender, IWebHostEnvironment environment, IDistributedCache cache)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _environment = environment;
            _tokenService = new TokenService(config, cache);
        }

        [HttpPost]
        [Route("Login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync([FromBody] string password)
        {
            var user = await _userManager.FindByNameAsync("admin");
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                //var roles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                //foreach (var role in roles) authClaims.Add(new Claim(ClaimTypes.Role, role));

                var jwtAccessToken = new JwtSecurityTokenHandler().WriteToken(_tokenService.GenerateNewAccessToken(authClaims));
                var refreshToken = _tokenService.GenerateRefreshToken();

                if (!await _tokenService.IsRefreshTokenExistsAsync(_adminSha512))
                    await _tokenService.SaveRefreshTokenToCacheAsync(_adminSha512, refreshToken);

                AppendCookie("AuthCookie", jwtAccessToken);

                return Ok();
            }
            return Unauthorized();
        }

        [Route("ValidateJWTToken")]
        public async Task<IActionResult> ValidateJwtTokenAsync()
        {
            var accessToken = Request.Cookies[_cookieName];

            if (string.IsNullOrEmpty(accessToken)) return Ok("No authorize");

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

                return Ok();
            }
            catch (SecurityTokenExpiredException ex)
            {
                Debug.WriteLine(ex.Message);

                if (await _tokenService.IsRefreshTokenExistsAsync(_adminSha512))
                {
                    var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

                    var newJwtAccessSecurityToken = _tokenService.GenerateNewAccessToken(principal.Claims.ToList());
                    AppendCookie(_cookieName, new JwtSecurityTokenHandler().WriteToken(newJwtAccessSecurityToken));

                    return Ok();
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
        [Route("RevokeRefreshToken")]
        public async Task<IActionResult> RevokeRefreshToken()
        {
            await _tokenService.IsRevokeRefreshTokenAsync(_adminSha512);

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("RevokeAllRefreshTokens")]
        public async Task<IActionResult> RevokeAllRefreshTokens()
        {
            var users = _userManager.Users.ToList();

            var userNames = new List<string>();
            var userNameHash = "";
            foreach (var user in users)
            {
                userNameHash = await ComputeSha512Async(Encoding.UTF8.GetBytes(user.UserName));
                userNames.Add(userNameHash);
            }

            await _tokenService.RevokeAllRefreshTokensAsync(userNames);

            return NoContent();
        }

        private static async Task<string> ComputeSha512Async(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var hashBytes = await SHA512.Create().ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        private void AppendCookie(string key, string value)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = !_environment.IsDevelopment(),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                IsEssential = true
            };

            Response.Cookies.Append(key, value, cookieOptions);
        }
    }
}
