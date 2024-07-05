using CookBook.Domain.Dishes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookBook.Persistence.Dishes
{
    public class IngredientsConfiguration : IEntityTypeConfiguration<Ingredients>
    {
        public void Configure(EntityTypeBuilder<Ingredients> builder)
        {
            builder.ToTable("Ingredients");
            builder.HasKey(p => p.IngredientId);
            builder.HasOne(p => p.Recipe).WithMany(p => p.Ingredients).HasForeignKey(p => p.RecipeId);
            builder.HasQueryFilter(p => p.Status == 1);
        }
    }
}
