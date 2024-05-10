using CookBook.Application.Interface.Persistence.Users;
using CookBook.Domain.Users;
using CookBook.Persistence.Shared;

namespace CookBook.Persistence.Users
{
    public class UserMasterRepository : Repository<UserMaster>, IUserMasterRepository
    {
        public UserMasterRepository(IDatabaseContext database) : base(database)
        {
        }
    }
}
