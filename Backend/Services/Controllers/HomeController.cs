﻿using CookBook.Application.Common.Models;
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
        [HttpGet("cuisine")]
        public async Task<List<Cuisine>> GetAllCuisine()
        {
            return await mediator.Send(new GetAllCuisineQuery());
        }

        [HttpPost("recipe")]
        public async Task<List<Recipe>> GetAllRecipe([FromBody]GetAllRecipeQuery recipeQuery)
        {
            return await mediator.Send(recipeQuery);
        }

        [HttpGet("recipe/{id}/ingredients")]
        public async Task<List<string>> GetRecipeIngredients(long id)
        {
            return await mediator.Send(new GetRecipeIngredientsQuery(id));
        }

        [HttpGet("recipe/{id}/instruction")]
        public async Task<List<RecipeInstruction>> GetRecipeInstructions(long id)
        {
            return await mediator.Send(new GetRecipeDirectionsQuery(id));
        }

    }
}