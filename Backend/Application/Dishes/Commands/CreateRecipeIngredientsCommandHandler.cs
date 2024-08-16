using CookBook.Application.Exceptions;
using CookBook.Application.Interface.Auth;
using CookBook.Application.Interface.Persistence;
using CookBook.Application.Interface.Persistence.Dishes;
using CookBook.Domain.Dishes;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Application.Dishes.Commands
{
    public record CreateRecipeIngredientsCommand(string Ingredients, long RecipeId) : IRequest<Unit>;

    public class CreateRecipeIngredientsCommandValidator : AbstractValidator<CreateRecipeIngredientsCommand>
    {
        public CreateRecipeIngredientsCommandValidator()
        {
            RuleFor(x => x.Ingredients).NotNull().NotEmpty();
            RuleFor(x => x.RecipeId).NotNull();
        }
    }

    public class CreateRecipeIngredientsCommandHandler : IRequestHandler<CreateRecipeIngredientsCommand, Unit>
    {
        private readonly IIngredientsRepository ingredientsRepository;
        private readonly IRecipeRepository recipeRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IValidator<CreateRecipeIngredientsCommand> ingredientsValidator;
        private readonly IUnitOfWork unitOfWork;

        public CreateRecipeIngredientsCommandHandler(IIngredientsRepository ingredientsRepository,
                                                     IRecipeRepository recipeRepository,
                                                     ICurrentUserService currentUserService, 
                                                     IValidator<CreateRecipeIngredientsCommand> ingredientsValidator,
                                                     IUnitOfWork unitOfWork)
        {
            this.ingredientsRepository = ingredientsRepository;
            this.recipeRepository = recipeRepository;
            this.currentUserService = currentUserService;
            this.ingredientsValidator = ingredientsValidator;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(CreateRecipeIngredientsCommand request, CancellationToken cancellationToken)
        {
            await ingredientsValidator.ValidateAndThrowAsync(request, cancellationToken);

            bool isRecipeValid = await recipeRepository.GetAllAsNoTracking()
                                .AnyAsync(x => x.RecipeId == request.RecipeId, cancellationToken);

            if (!isRecipeValid)
                throw new BadRequestException("BE020301");

            long userId = currentUserService.GetUser().UserId;

            List<string> ingredients = request.Ingredients
                                      .Split('$', StringSplitOptions.RemoveEmptyEntries).ToList();

            List<Ingredients> recipeIngredients = new();

            foreach (string ingredient in ingredients)
            {
                Ingredients recipeIngredient = new()
                {
                    RecipeId = request.RecipeId,
                    UserId = userId,
                    Ingredient = ingredient.Trim(),
                    Status = 1,
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now,
                    ModifiedBy = userId,
                    ModifiedOn = DateTime.Now
                };
                recipeIngredients.Add(recipeIngredient);
            }

            await ingredientsRepository.AddRangeAsync(recipeIngredients);
            await unitOfWork.SaveAsync();
            return Unit.Value;
        }
    }
}
