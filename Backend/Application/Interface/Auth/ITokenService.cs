using System.Security.Claims;

namespace CookBook.Application.Interface.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
    }
}
