using CookBook.Application.Interface.Persistence.Dishes;
using CookBook.Domain.Dishes;
using CookBook.Persistence.Shared;

namespace CookBook.Persistence.Dishes
{
    public class RecipeRepository : Repository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(IDatabaseContext database) : base(database)
        {
        }
    }
}
