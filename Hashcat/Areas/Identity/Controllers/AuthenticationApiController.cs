using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebHashcat.Models;

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

        public AuthenticationApiController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IConfiguration config, IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _config = config;
            _emailSender = emailSender;
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

        [HttpPost]
        [Route("Login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(Login login)
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

                    var authStringKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:secret"]));
                    var token = new JwtSecurityToken(issuer: _config["JWT:validIssuer"],
                        audience: _config["JWT:validAudience"],
                        expires: DateTime.Now.AddDays(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authStringKey, SecurityAlgorithms.HmacSha512));

                    return Ok(new
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Response = new Response() { Status = "Success", Message = "Authenticated" }
                    });
                }
            }
            return Unauthorized();
            //return StatusCode(500, new Response() { Status = "Error", Message = "User authorization failed" });
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPassword model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var link = Url.Action("ResetPassword", "Account", new { Area = "Identity", token, userEmail = user.Email }, Request.Scheme, Request.Host.Value);
            await _emailSender.SendEmailAsync(user.Email, "Reset password", $"<a href = {link}>Click to reset password</a>");
            return Ok();
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout(string returnUrl = "")
        {
            returnUrl ??= Url.Content("~/");

            await _signInManager.SignOutAsync();

            return Ok();

            //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            //return RedirectToAction("Index", "Ad", new { Area = "" });
        }
    }
}