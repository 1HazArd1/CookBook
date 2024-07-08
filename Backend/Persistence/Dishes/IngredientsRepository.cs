using CookBook.Application.Interface.Persistence.Dishes;
using CookBook.Domain.Dishes;
using CookBook.Persistence.Shared;

namespace CookBook.Persistence.Dishes
{
    internal class IngredientsRepository : Repository<Ingredients>, IIngredientsRepository
    {
        public IngredientsRepository(IDatabaseContext database) : base(database)
        {
        }
    }
}
