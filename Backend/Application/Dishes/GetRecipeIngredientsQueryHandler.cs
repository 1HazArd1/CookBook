using CookBook.Application.Common.Models;
using CookBook.Application.Interface.Auth;
using CookBook.Application.Interface.Persistence.Dishes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Application.Dishes
{
    public record GetRecipeIngredientsQuery(long RecipeId) : IRequest<List<string>>;
    public class GetRecipeIngredientsQueryHandler : IRequestHandler<GetRecipeIngredientsQuery, List<string>>
    {
        private readonly IIngredientsRepository ingredientsRepository;
        private readonly ICurrentUserService currentUserService;

        public GetRecipeIngredientsQueryHandler(IIngredientsRepository ingredientsRepository,
                                                ICurrentUserService currentUserService)
        {
            this.ingredientsRepository = ingredientsRepository;
            this.currentUserService = currentUserService;
        }
        public async Task<List<string>> Handle(GetRecipeIngredientsQuery request, CancellationToken cancellationToken)
        {
            long userId = currentUserService.GetUser().UserId;

            List<string> ingredients = await ingredientsRepository.GetAllAsNoTracking()
                                      .Where(x => x.UserId == userId && x.RecipeId == request.RecipeId)
                                      .Select(x => x.Ingredient)
                                      .ToListAsync(cancellationToken);

            return ingredients;
        }
    }
}
