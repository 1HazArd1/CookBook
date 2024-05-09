using CookBook.Domain.Dishes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookBook.Persistence.Dishes
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.ToTable("Recipe");
            builder.HasKey(p => p.RecipeId);
            builder.HasQueryFilter(p => p.Status == 1);
        }
    }
}
