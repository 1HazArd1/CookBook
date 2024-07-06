using CookBook.Application.Common.Models;
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
    public record CreateRecipeCommand(Common.Models.Recipe Recipe) : IRequest<Unit>;
    public record CreateRecipeIngredientsCommand(string Ingredients) : IRequest<Unit>;
    public record CreateRecipeInstructionsCommand(List<UserRecipeInstruction> RecipeInstructions) : IRequest<Unit>;

    public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
    {
        public CreateRecipeCommandValidator() 
        {
            RuleFor(x => x.Recipe.Name).NotNull().NotEmpty();
        }
    }

    public class CreateRecipeIngredientsCommandValidator : AbstractValidator<CreateRecipeIngredientsCommand>
    {
        public CreateRecipeIngredientsCommandValidator()
        {
            RuleFor(x => x.Ingredients).NotNull().NotEmpty();
        }
    }
    public class CreateRecipeInstructionsCommandValidator : AbstractValidator<CreateRecipeInstructionsCommand>
    {
        public CreateRecipeInstructionsCommandValidator() 
        { 
            RuleFor(x => x.RecipeInstructions)
                    .Must(instructions => instructions.Select(y => y.Component.ToLower().Trim()).Distinct().Count() == instructions.Count)
                    .WithMessage("Duplicate component names are not allowed");
        }

    }
    public class CreateUserRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Unit>, IRequestHandler<CreateRecipeInstructionsCommand, Unit>, IRequestHandler<CreateRecipeIngredientsCommand, Unit>
    {
        private readonly IRecipeRepository recipeRepository;
        private readonly IComponentRepository componentRepository;
        private readonly IDirectionRepository directionRepository;
        private readonly IIngredientsRepository ingredientsRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IValidator<CreateRecipeCommand> recipeValidator;
        private readonly IValidator<CreateRecipeInstructionsCommand> instructionsValidator;
        private readonly IValidator<CreateRecipeIngredientsCommand> ingredientsValidator;
        private long recipeId = 1;

        public CreateUserRecipeCommandHandler(IRecipeRepository recipeRepository,
                                              IComponentRepository componentRepository,
                                              IDirectionRepository directionRepository,
                                              IIngredientsRepository ingredientsRepository,
                                              ICurrentUserService currentUserService,
                                              IUnitOfWork unitOfWork,
                                              IValidator<CreateRecipeCommand> recipeValidator,
                                              IValidator<CreateRecipeIngredientsCommand> ingredientsValidator,
                                              IValidator<CreateRecipeInstructionsCommand> instructionsValidator)
        {
            this.recipeRepository = recipeRepository;
            this.componentRepository = componentRepository;
            this.directionRepository = directionRepository;
            this.ingredientsRepository = ingredientsRepository;
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;
            this.recipeValidator = recipeValidator;
            this.instructionsValidator = instructionsValidator;
            this.ingredientsValidator = ingredientsValidator;
        }

        public long RecipeId { get => recipeId; set => recipeId = value; }

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

            recipeId = userRecipe.RecipeId;

            return Unit.Value;
        }

        public async Task<Unit> Handle(CreateRecipeIngredientsCommand request, CancellationToken cancellationToken)
        {
            await ingredientsValidator.ValidateAndThrowAsync(request, cancellationToken);

            long userId = currentUserService.GetUser().UserId;

            List<string> ingredients = request.Ingredients
                                      .Split('$', StringSplitOptions.RemoveEmptyEntries).ToList();

            List<Ingredients> recipeIngredients = new();

            foreach (string ingredient in ingredients)
            {
                Ingredients recipeIngredient = new()
                {
                    RecipeId = recipeId,
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


        public async Task<Unit> Handle(CreateRecipeInstructionsCommand request, CancellationToken cancellationToken)
        {
            await instructionsValidator.ValidateAndThrowAsync(request, cancellationToken);

            long userId = currentUserService.GetUser().UserId;

            byte componentStepNo = 0;
            byte instructionStepNo = 0;

            List<Component> components = new();
            List<Direction> directions = new();

            List<string> componentNames = request.RecipeInstructions.Select(x => x.Component).ToList();

            foreach (UserRecipeInstruction userRecipeInstruction in request.RecipeInstructions)
            {
                componentStepNo++;
                components.Add(CreateRecipeComponent(componentStepNo, userRecipeInstruction.Component, userId));
            }
            await componentRepository.AddRangeAsync(components);
            await unitOfWork.SaveAsync();

            Dictionary<string, long> componentMapping = await componentRepository.GetAllAsNoTracking()
                                                       .Where(x => x.UserId == userId && x.RecipeId == recipeId && componentNames.Contains(x.ComponentName))
                                                       .ToDictionaryAsync(x => x.ComponentName, x => x.ComponentId, cancellationToken);

            foreach (UserRecipeInstruction userRecipeInstruction in request.RecipeInstructions)
            {
                List<string> recipeInstructions = userRecipeInstruction.Directions
                                                 .Split('$', StringSplitOptions.RemoveEmptyEntries).ToList();

                long componentId = componentMapping.GetValueOrDefault(userRecipeInstruction.Component);

                if (componentId == 0)
                    continue;

                foreach (string recipeInstruction in recipeInstructions)
                {
                    instructionStepNo++;
                    directions.Add(CreateRecipeInstruction(instructionStepNo, componentId, recipeInstruction, userId));
                }
            }
            await directionRepository.AddRangeAsync(directions);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }

        private Component CreateRecipeComponent(byte stepNo, string componentName, long userId)
        {
            Component recipeComponent = new()
            {
                UserId = userId,
                RecipeId = recipeId,
                StepNo = stepNo,
                ComponentName = componentName.Trim(),
                Status = 1,
                CreatedBy = userId,
                CreatedOn = DateTime.Now,
                ModifiedBy = userId,
                ModifiedOn = DateTime.Now
            };

            return recipeComponent;
        }

        private static Direction CreateRecipeInstruction(byte stepNo, long componentId, string instruction, long userId)
        {
            Direction recipeInstruction = new()
            {
                UserId = userId,
                ComponentId = componentId,
                StepNo = stepNo,
                Instruction = instruction.Trim(),
                Status = 1,
                CreatedBy = userId,
                CreatedOn = DateTime.Now,
                ModifiedBy = userId,
                ModifiedOn = DateTime.Now
            };

            return recipeInstruction;
        }
    }
}