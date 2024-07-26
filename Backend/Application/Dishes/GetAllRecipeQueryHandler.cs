using CookBook.Application.Common;
using CookBook.Application.Common.Models;
using CookBook.Application.Interface.Auth;
using CookBook.Application.Interface.Persistence.Dishes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Application.Dishes
{
    public record GetAllRecipeQuery(String? SearchText) : IRequest<List<Recipe>>;

    public class GetAllRecipeQueryHandler : IRequestHandler<GetAllRecipeQuery, List<Recipe>>
    {
        private readonly IRecipeRepository recipeRepository;
        private readonly ICurrentUserService currentUserService;

        public GetAllRecipeQueryHandler(IRecipeRepository recipeRepository,
                                        ICurrentUserService currentUserService)
        {
            this.recipeRepository = recipeRepository;
            this.currentUserService = currentUserService;
        }
        public async Task<List<Recipe>> Handle(GetAllRecipeQuery request, CancellationToken cancellationToken)
        {
            LoggedInUser loggedInUser = currentUserService.GetUser();

            List<Recipe> recipes = await recipeRepository.GetAllAsNoTracking()
                                   .Where(x => x.UserId == loggedInUser.UserId || x.UserId == Global.AdminUserId)
                                   .Select(x => new Recipe
                                   {
                                       Id = x.RecipeId,
                                       Name = x.RecipeName,
                                       RecipeUrl = x.RecipeUrl,
                                       Cuisine = x.Cuisine,
                                       Duration = x.Duration,
                                       Servings = x.Servings,
                                       IsEditable = x.UserId == loggedInUser.UserId
                                   }).ToListAsync(cancellationToken);


            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                string searchText = request.SearchText.Trim();
                recipes = recipes.Where(x => x.Name.Equals(request.SearchText,StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return recipes;
        }
    }
}
