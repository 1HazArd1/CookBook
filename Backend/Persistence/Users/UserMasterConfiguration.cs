using iMocha.Talent.Analytics.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMocha.Talent.Analytics.Persistence.Users
{
    public class UserMasterConfiguration : IEntityTypeConfiguration<UserMaster>
    {
        public void Configure(EntityTypeBuilder<UserMaster> builder)
        {
            builder.ToTable("UserMaster");
            builder.HasKey(p => p.UserId);
        }
    }
}
