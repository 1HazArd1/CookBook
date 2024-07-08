using CookBook.Application.Common.Models;
using CookBook.Application.Dishes.Commands;
using CookBook.Application.Dishes;
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
        public async Task<IActionResult> CreateUserRecipe([FromBody]Recipe recipe)
        {
            await mediator.Send(new CreateRecipeCommand(recipe));
            return Ok();
        }

        [HttpPost("Ingredients")]
        public async Task<IActionResult> CreateRecipeIngredients([FromBody] string ingredients)
        {
            await mediator.Send(new CreateRecipeIngredientsCommand(ingredients));
            return Ok();
        }

        [HttpPost("Instruction")]
        public async Task<IActionResult> CreateRecipeInstruction(List<UserRecipeInstruction> recipeInstructions)
        {
            await mediator.Send(new CreateRecipeInstructionsCommand(recipeInstructions));
            return Ok();
        }
    }
}
