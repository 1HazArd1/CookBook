
using iMocha.Talent.Analytics.Domain.Common;

namespace iMocha.Talent.Analytics.Domain.Users
{
    public class UserMaster : IEntity
    {
        public long UserId { get; set; }
        public long CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }

    }
}
