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
    }
}
