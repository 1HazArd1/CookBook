using CookBook.Application.Interface.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CookBook.Services.Services
{
    public class JWTTokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public JWTTokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            int tokenValidityInMinutes = int.Parse(configuration["JWT:TokenValidityMinutes"]!);
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new (
                                                  configuration["JWT:Issuer"],
                                                  configuration["JWT:Audience"],
                                                  claims: claims,
                                                  notBefore: DateTime.Now,
                                                  expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                                                  signingCredentials: credentials
                                              );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return jwtToken;
        }
    }
}
