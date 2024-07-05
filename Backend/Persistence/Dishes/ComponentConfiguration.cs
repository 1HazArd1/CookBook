using CookBook.Domain.Dishes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookBook.Persistence.Dishes
{
    public class ComponentConfiguration : IEntityTypeConfiguration<Component>
    {
        public void Configure(EntityTypeBuilder<Component> builder)
        {
            builder.ToTable("Component");
            builder.HasKey(p => p.ComponentId);
            builder.HasOne(p => p.Recipe).WithMany(p => p.Components).HasForeignKey(p => p.RecipeId);
            builder.HasMany(p => p.Directions).WithOne(p => p.Component).HasForeignKey(p => p.ComponentId);
            builder.HasQueryFilter(p => p.Status == 1);
        }
    }
}
