using CookBook.Application.Common.Models;
using CookBook.Application.Dishes;
using CookBook.Application.Dishes.Commands;
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

        [HttpPost("Recipe")]
        public async Task<IActionResult> CreateUserRecipe(Recipe recipe)
        {
            await mediator.Send(new CreateRecipeCommand(recipe));
            return Ok();
        }

        [HttpPost("Recipe/Instruction")]
        public async Task<IActionResult> CreateRecipeInstruction(List<RecipeInstruction> recipeInstructions)
        {
            await mediator.Send(new CreateRecipeInstructionCommand(recipeInstructions));
            return Ok();
        }
    }
}