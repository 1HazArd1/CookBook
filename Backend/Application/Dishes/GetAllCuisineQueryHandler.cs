using CookBook.Application.Common.Models;
using CookBook.Application.Interface.Persistence.Dishes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Application.Dishes
{
    public record GetAllCuisineQuery() : IRequest<List<Cuisine>>;
    public class GetAllCuisineQueryHandler : IRequestHandler<GetAllCuisineQuery, List<Cuisine>>
    {
        private readonly ICuisineRepository cuisineRepository;

        public GetAllCuisineQueryHandler(ICuisineRepository cuisineRepository)
        {
            this.cuisineRepository = cuisineRepository;
        }
        public async Task<List<Cuisine>> Handle(GetAllCuisineQuery request, CancellationToken cancellationToken)
        {
            List<Cuisine> cuisines = await cuisineRepository.GetAllAsNoTracking()
                                     .Select(x => new Cuisine
                                     {
                                         CuisineId = x.CuisineId,
                                         CuisineName = x.CuisineName,
                                         CuisineUrl = x.CuisineUrl
                                     }).ToListAsync(cancellationToken);

            return cuisines;
        }
    }
}
