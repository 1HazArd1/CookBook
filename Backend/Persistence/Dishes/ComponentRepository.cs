using CookBook.Application.Interface.Persistence.Dishes;
using CookBook.Domain.Dishes;
using CookBook.Persistence.Shared;

namespace CookBook.Persistence.Dishes
{
    public class ComponentRepository : Repository<Component>, IComponentRepository
    {
        public ComponentRepository(IDatabaseContext database) : base(database)
        {
        }
    }
}
