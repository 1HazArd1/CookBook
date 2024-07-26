using CookBook.Application.Interface.Persistence.Dishes;
using CookBook.Domain.Dishes;
using CookBook.Persistence.Shared;

namespace CookBook.Persistence.Dishes
{
    public class CuisineRepository : Repository<Cuisine>, ICuisineRepository
    {
        public CuisineRepository(IDatabaseContext database) : base(database)
        {
        }
    }
}
