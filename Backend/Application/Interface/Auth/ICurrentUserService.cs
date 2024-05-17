using CookBook.Application.Common.Models;

namespace CookBook.Application.Interface.Auth
{
    public interface ICurrentUserService
    {
        LoggedInUser GetUser();
    }
}
