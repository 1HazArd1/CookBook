using CookBook.Application.Exceptions;
using CookBook.Application.Interface.Auth;
using CookBook.Application.Interface.Persistence;
using CookBook.Application.Interface.Persistence.Dishes;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Application.Dishes.Commands
{
    public record CreateRecipeCommand(Common.Models.Recipe Recipe) : IRequest<Unit>;

    public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
    {
        public CreateRecipeCommandValidator() 
        {
            RuleFor(x => x.Recipe.Name).NotNull().NotEmpty();
        }
    }

    public class CreateUserRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Unit>
    {
        private readonly IRecipeRepository recipeRepository;
        private readonly IIngredientsRepository ingredientsRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IValidator<CreateRecipeCommand> recipeValidator;

        public CreateUserRecipeCommandHandler(IRecipeRepository recipeRepository,
                                              IIngredientsRepository ingredientsRepository,
                                              ICurrentUserService currentUserService,
                                              IUnitOfWork unitOfWork,
                                              IValidator<CreateRecipeCommand> recipeValidator)
        {
            this.recipeRepository = recipeRepository;
            this.ingredientsRepository = ingredientsRepository;
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;
            this.recipeValidator = recipeValidator;
        }

        public async Task<Unit> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
        {
            await recipeValidator.ValidateAndThrowAsync(request, cancellationToken);

            long userId = currentUserService.GetUser().UserId;

            Domain.Dishes.Recipe? existingRecipe = await recipeRepository.GetAllAsNoTracking()
                                                  .Where(x => x.UserId == userId && x.RecipeName.ToLower() == request.Recipe.Name.Trim().ToLower())
                                                  .FirstOrDefaultAsync(cancellationToken);

            if (existingRecipe != null)
                throw new BadRequestException("BE020101");

            Domain.Dishes.Recipe userRecipe = new()
            {
                RecipeName = request.Recipe.Name,
                Cuisine = request.Recipe.Cuisine,
                RecipeUrl = request.Recipe.RecipeUrl,
                UserId = userId,
                Duration = request.Recipe.Duration,
                Servings = request.Recipe.Servings,
                Status = 1,
                CreatedBy = userId,
                CreatedOn = DateTime.Now,
                ModifiedBy = userId,
                ModifiedOn = DateTime.Now
            };

            await recipeRepository.AddAsync(userRecipe);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }

    }
}