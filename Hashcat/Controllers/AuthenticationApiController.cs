using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebHashcat.Models;

namespace WebHashcat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;

        public AuthenticationApiController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register register)
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

            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var confirmLink = Url.Action("Confirm", "EmailConfirm", new { guid = token, userEmail = user.Email }, Request.Scheme, Request.Host.Value);

            //await _emailSender.SendEmailAsync(user.Email, "Please activate link", $"<a href = {confirmLink}>Click to confirm email</a>");

            //var res = await _userManager.ConfirmEmailAsync(user, token);

            var res = await _userManager.CreateAsync(user, register.Password);
            return !res.Succeeded ? new BadRequestObjectResult(res) : StatusCode(201);

            //if (!res.Succeeded) return StatusCode(500, new Response() { Status = "Error", Message = "User create failed" });

            //return Ok(new Response() { Status = "Success", Message = "User created" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(Login login)
        {
            if (login != null)
            {
                var user = await _userManager.FindByEmailAsync(login.Login_);
                if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    foreach (var role in roles) authClaims.Add(new Claim(ClaimTypes.Role, role));

                    var authStringKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
                    var token = new JwtSecurityToken(issuer: _config["JWT:ValidAudience"],
                        expires: DateTime.Now.AddDays(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authStringKey, SecurityAlgorithms.HmacSha512));

                    return Ok(new
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Response = new Response() { Status = "Success", Message = "Authorizated" }
                    });
                }
            }
            return Unauthorized();
            //return StatusCode(500, new Response() { Status = "Error", Message = "User authorization failed" });
        }
    }
}
