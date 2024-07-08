using CookBook.Domain.Dishes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookBook.Persistence.Dishes
{
    public class DirectionConfiguration : IEntityTypeConfiguration<Direction>
    {
        public void Configure(EntityTypeBuilder<Direction> builder)
        {
            builder.ToTable("Direction");
            builder.HasKey(p => p.DirectionId);
            builder.HasOne(p => p.Component).WithMany(p => p.Directions).HasForeignKey(p => p.ComponentId);
            builder.HasQueryFilter(p => p.Status == 1);
        }
    }
}
