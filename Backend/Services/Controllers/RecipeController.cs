using CookBook.Application.Common.Models;
using CookBook.Application.Dishes.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CookBook.Services.Controllers
{
    public class RecipeController :ApiAuthorizeControllerBase
    {
        private readonly IMediator mediator;

        public RecipeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<long> CreateUserRecipe([FromBody]Recipe recipe)
        {
            return await mediator.Send(new CreateUserRecipeCommand(recipe));
        }

        [HttpPost("{id}/ingredients")]
        public async Task<IActionResult> CreateRecipeIngredients([FromBody] string ingredients, long id)
        {
            await mediator.Send(new CreateRecipeIngredientsCommand(ingredients, id));
            return Ok();
        }

        [HttpPost("{id}/instruction")]
        public async Task<IActionResult> CreateRecipeInstruction(List<UserRecipeInstruction> recipeInstructions, long id)
        {
            await mediator.Send(new CreateRecipeInstructionsCommand(recipeInstructions, id));
            return Ok();
        }
    }
}
