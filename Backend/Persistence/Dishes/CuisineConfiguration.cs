using CookBook.Domain.Dishes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookBook.Persistence.Dishes
{
    public class CuisineConfiguration : IEntityTypeConfiguration<Cuisine>
    {
        public void Configure(EntityTypeBuilder<Cuisine> builder)
        {
            builder.ToTable("Cuisine");
            builder.HasKey(p => p.CuisineId);
            builder.HasQueryFilter(p => p.Status == 1);
        }
    }
}
