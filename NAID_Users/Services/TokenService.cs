using Microsoft.IdentityModel.Tokens;
using NAID_Users.Interfaces;
using NAID_Users.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NAID_Users.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _Config;
        private readonly SymmetricSecurityKey _Key;
        public TokenService(IConfiguration config)
        {
            _Config = config;
            _Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Config["JWt:SigninKey"]));
        }
        public string CreateToken(AppUser appUser, string role)
        {
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, appUser.UserName),
                new Claim(ClaimTypes.Role,role),new Claim(ClaimTypes.Role,role),
                new Claim(JwtRegisteredClaimNames.Sub, appUser.Id),
            };

            var credentials = new SigningCredentials(_Key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDiscriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentials,
                Issuer = _Config["JWT:Issuer"],
                Audience = _Config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDiscriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
