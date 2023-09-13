using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebHashcat.Areas.Cabinet.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public BalanceApiController(UserManager<User> userManager) => _userManager = userManager;

        [HttpPost]
        [Route("Replenishment")]
        public async Task<IActionResult> Replenishment([FromBody] decimal sum)
        {
            var token = HttpContext.Request.Cookies["AuthCookie"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
            var userName = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;

            var currentUser = await _userManager.FindByNameAsync(userName);
            currentUser.Balance += sum;
            await _userManager.UpdateAsync(currentUser);

            return Ok(currentUser.Balance);
        }

        //[Route("Test")]
        //public IActionResult Test()
        //{
        //    string filePath = "hashcat-6.2.6\\hashcat.exe";

        //    if (System.IO.File.Exists(filePath))
        //    {
        //        return Ok("Файл існує.");
        //    }
        //    else
        //    {
        //        return Ok("Файл не існує.");
        //    }
        //}
    }
}