using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace WebHashcat.Configurations
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomUserIdProvider(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        public virtual string? GetUserId(HubConnectionContext connection)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthCookie"];
            if (!string.IsNullOrEmpty(token))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
                var userName = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
                return ComputeSha512(Encoding.UTF8.GetBytes(userName));
            }
            return null;
        }

        private string ComputeSha512(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var hashBytes = SHA512.Create().ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
