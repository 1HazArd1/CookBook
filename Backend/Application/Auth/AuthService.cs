using CookBook.Application.Interface.Auth;
using CookBook.Domain.Users;
using System.Security.Claims;

namespace CookBook.Application.Auth
{
    public interface IAuthService
    {
        AuthSession GetAuthSession(UserMaster user);
    }
    public record AuthSession (long UserId, string Email, string FirstName, string? LastName, string AccessToken);

    //service providing the creation of JWT token with the required parameters
    public class AuthService : IAuthService
    {
        private readonly ITokenService tokenService;

        public AuthService(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }
        public AuthSession GetAuthSession(UserMaster user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.UserId.ToString())
            };
            string token = tokenService.GenerateAccessToken(claims);

            return new AuthSession(user.UserId,user.Email, user.FirstName, user.LastName, token);
        }
    }
}
    