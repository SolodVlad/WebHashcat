using BLL.Services;
using Domain.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WebHashcat.Areas.Identity.Models;
using WebHashcat.Models;

namespace WebHashcat.Areas.Identity.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly IDistributedCache _cache;

        public TokenService(IConfiguration config, IDistributedCache cache)
        {
            _config = config;
            _cache = cache;
        }

        public async Task SaveRefreshTokenToCacheAsync(string key, string value)
        {
            await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(int.Parse(_config["JWT:refreshTokenValidityInDays"]))
                //AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        public async Task<bool> IsRefreshTokenExistsAsync(string key) => !string.IsNullOrEmpty(await _cache.GetStringAsync(key));

        //[HttpPost]
        //[Route("Refresh-token")]
//        public async Task<string> RefreshTokens(Tokens tokens)
//        {
//            if (tokens is null) throw new NullReferenceException("Invalid client request");

//            string accessToken = tokens.AccessToken;
//            string refreshToken = tokens.RefreshToken;

//            var principal = GetPrincipalFromExpiredToken(accessToken) ?? throw new SecurityTokenException("Invalid access token or refresh token");

//#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
//#pragma warning disable CS8602 // Dereference of a possibly null reference.
//            string username = principal.Identity.Name;
//#pragma warning restore CS8602 // Dereference of a possibly null reference.
//#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

//            var user = await _userManager.FindByNameAsync(username);

//            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
//                throw new SecurityTokenException("Invalid access token or refresh token");

//            var newAccessToken = GenerateAccessToken(principal.Claims.ToList());
//            var newRefreshToken = GenerateRefreshToken();

//            user.RefreshToken = newRefreshToken;
//            await _userManager.UpdateAsync(user);

//            return new JwtSecurityTokenHandler().WriteToken(newAccessToken);

//            //return Ok(new
//            //{
//            //    accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
//            //    //refreshToken = newRefreshToken
//            //});
//        }

        public JwtSecurityToken GenerateNewAccessToken(List<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("JWTSecret")));
            _ = int.TryParse(_config["JWT:tokenValidityInMinutes"], out int tokenValidityInMinutes);

            var jtiClaim = claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti);

            if (jtiClaim != null) claims.Remove(jtiClaim);

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var token = new JwtSecurityToken(
                    issuer: _config["JWT:validIssuer"],
                    audience: _config["JWT:validAudience"],
                    expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512)
                );

            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("JWTSecret"))),
                ValidateLifetime = false,
                ValidIssuer = _config["JWT:validIssuer"],
                ValidAudience = _config["JWT:validAudience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task<bool> IsRevokeRefreshTokenAsync(string key)
        {
            if (string.IsNullOrEmpty(await _cache.GetStringAsync(key))) return false;

            await _cache.RemoveAsync(key);

            return true;
        }

        public async Task RevokeAllRefreshTokensAsync(List<string> keys)
        {
            foreach (var key in keys) await _cache.RemoveAsync(key);
        }
    }
}
