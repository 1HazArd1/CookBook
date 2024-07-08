using CookBook.Application.Common.Models;
using CookBook.Application.Interface.Auth;
using CookBook.Application.Interface.Persistence.Dishes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Application.Dishes
{
    public record GetRecipeDirectionsQuery(long RecipeId) : IRequest<List<RecipeInstruction>>;
    public class GetRecipeDirectionsQueryHandler : IRequestHandler<GetRecipeDirectionsQuery, List<RecipeInstruction>>
    {
        private readonly IComponentRepository componentRepository;
        private readonly ICurrentUserService currentUserService;

        public GetRecipeDirectionsQueryHandler(IComponentRepository componentRepository,
                                     ICurrentUserService currentUserService)
        {
            this.componentRepository = componentRepository;
            this.currentUserService = currentUserService;
        }

        public async Task<List<RecipeInstruction>> Handle(GetRecipeDirectionsQuery request, CancellationToken cancellationToken)
        {
            LoggedInUser loggedInUser = currentUserService.GetUser();

            List<RecipeInstruction> recipeDirections = await componentRepository.GetAllAsNoTracking()
                                                       .Where(x => x.RecipeId == request.RecipeId && x.UserId == loggedInUser.UserId)
                                                       .Select(x => new RecipeInstruction
                                                       {
                                                           Component = x.ComponentName,
                                                           Directions = x.Directions.Select(y => y.Instruction).ToList(),
                                                       }).ToListAsync(cancellationToken);

            return recipeDirections;
        }
    }
}
