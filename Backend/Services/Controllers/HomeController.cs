using CookBook.Application.Common.Models;
using CookBook.Application.Dishes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CookBook.Services.Controllers
{
    public class HomeController : ApiAuthorizeControllerBase
    {
        private readonly IMediator mediator;

        public HomeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("Recipe")]
        public async Task<List<Recipe>> GetAllRecipe()
        {
            return await mediator.Send(new GetAllRecipeQuery());
        }

        [HttpGet("Recipe/{id}/Ingredients")]
        public async Task<List<string>> GetRecipeIngredients(long id)
        {
            return await mediator.Send(new  GetRecipeIngredientsQuery(id));
        }

        [HttpGet("Recipe/{id}/Instruction")]
        public async Task<List<RecipeInstruction>> GetRecipeInstructions(long id)
        {
            return await mediator.Send(new GetRecipeDirectionsQuery(id));
        }

    }
}