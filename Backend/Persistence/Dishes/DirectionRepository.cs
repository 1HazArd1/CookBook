using CookBook.Application.Interface.Persistence.Dishes;
using CookBook.Domain.Dishes;
using CookBook.Persistence.Shared;

namespace CookBook.Persistence.Dishes
{
    public class DirectionRepository : Repository<Direction>, IDirectionRepository
    {
        public DirectionRepository(IDatabaseContext database) : base(database)
        {
        }
    }
}
