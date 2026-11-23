using E_Commerce.Application.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity.Services
{
    // DS_TOIKHKVHHQTI
    internal class TokenService : ITokenService
    {
        private readonly JWTSettings _jwtSettings;

        public TokenService(IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }
        public string CreateToken(string userId, string email, string userName, IEnumerable<string> roles)
        {
            // claims 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, userName)
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // sign in credintials  [secret key , security algorithm]
            var secKey = _jwtSettings.SecretKey;
            if (string.IsNullOrWhiteSpace(secKey))
                throw new InvalidOperationException("JWT Secret key is not configured.");
            if(secKey.Length < 32)
                throw new InvalidOperationException("JWT Secret Key is too Short.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // issuer ,audience, expires,
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    public class JWTSettings
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int ExpirationInMinutes { get; set; }
    }
}
