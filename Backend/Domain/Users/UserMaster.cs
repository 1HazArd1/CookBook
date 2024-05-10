
using CookBook.Domain.Common;

namespace CookBook.Domain.Users
{
    public class UserMaster : IEntity
    {
        public long UserId { get; set; }
        //public long CustomerId { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte Status {  get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
