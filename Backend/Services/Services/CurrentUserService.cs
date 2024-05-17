using CookBook.Application.Common.Models;
using CookBook.Application.Interface.Auth;

namespace CookBook.Services.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public LoggedInUser GetUser()
        {
            LoggedInUser? loggedInUser = null;

            var userIdValue = httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
            var emailClaimValue = httpContextAccessor.HttpContext?.User.FindFirst("Email")?.Value;

            if (long.TryParse(userIdValue, out long UserId) && !string.IsNullOrEmpty(emailClaimValue)) 
            {
                loggedInUser = new LoggedInUser ( UserId: UserId, Email: emailClaimValue );
            }

            if (loggedInUser == null)
                throw new UnauthorizedAccessException("User not authorized");

            return loggedInUser;
        }
    }
}
