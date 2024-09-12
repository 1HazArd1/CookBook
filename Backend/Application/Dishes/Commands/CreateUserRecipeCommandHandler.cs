using CookBook.Application.Exceptions;
using CookBook.Application.Interface.Auth;
using CookBook.Application.Interface.Persistence;
using CookBook.Application.Interface.Persistence.Dishes;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Xml.Linq;

namespace CookBook.Application.Dishes.Commands
{
    public record CreateUserRecipeCommand(Common.Models.Recipe Recipe) : IRequest<long>;

    public class CreateUserRecipeCommandValidator : AbstractValidator<CreateUserRecipeCommand>
    {
        public CreateUserRecipeCommandValidator() 
        {
            RuleFor(x => x.Recipe.Name).NotNull().NotEmpty();
        }
    }

    public class CreateUserRecipeCommandHandler : IRequestHandler<CreateUserRecipeCommand, long>
    {
        private readonly IRecipeRepository recipeRepository;
        private readonly IIngredientsRepository ingredientsRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IValidator<CreateUserRecipeCommand> recipeValidator;

        public CreateUserRecipeCommandHandler(IRecipeRepository recipeRepository,
                                              IIngredientsRepository ingredientsRepository,
                                              ICurrentUserService currentUserService,
                                              IUnitOfWork unitOfWork,
                                              IValidator<CreateUserRecipeCommand> recipeValidator)
        {
            this.recipeRepository = recipeRepository;
            this.ingredientsRepository = ingredientsRepository;
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;
            this.recipeValidator = recipeValidator;
        }

        public async Task<long> Handle(CreateUserRecipeCommand request, CancellationToken cancellationToken)
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
                RecipeName = ConvertTitleCase(request.Recipe.Name),
                Cuisine = request.Recipe.Cuisine == null ? null : ConvertTitleCase(request.Recipe.Cuisine),
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

            return userRecipe.RecipeId;

        }

        private static string ConvertTitleCase(string unformattedString)
        {
            // Create a TextInfo object based on the current culture.
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            // Convert to title case (first letter of each word capitalized)
            string tittleCaseString = textInfo.ToTitleCase(unformattedString.ToLower());

            return tittleCaseString;
        }

    }
}