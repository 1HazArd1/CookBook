using iMocha.Talent.Analytics.Application.Interface.Persistence.Users;
using iMocha.Talent.Analytics.Domain.Users;
using iMocha.Talent.Analytics.Persistence.Shared;

namespace iMocha.Talent.Analytics.Persistence.Users
{
    public class UserMasterRepository : Repository<UserMaster>, IUserMasterRepository
    {
        public UserMasterRepository(IDatabaseContext database) : base(database)
        {
        }
    }
}
