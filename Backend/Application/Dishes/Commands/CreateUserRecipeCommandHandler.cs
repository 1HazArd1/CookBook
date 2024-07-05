using CookBook.Application.Common.Models;
using CookBook.Application.Exceptions;
using CookBook.Application.Interface.Auth;
using CookBook.Application.Interface.Persistence;
using CookBook.Application.Interface.Persistence.Dishes;
using CookBook.Domain.Dishes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Application.Dishes.Commands
{
    public record CreateRecipeCommand(Common.Models.Recipe Recipe) : IRequest<Unit>;
    public record CreateRecipeInstructionsCommand(List<UserRecipeInstruction> RecipeInstructions) : IRequest<Unit>;
    public record CreateRecipeIngredientsCommand(string Ingredients) : IRequest<Unit>;
    public class CreateUserRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Unit>, IRequestHandler<CreateRecipeInstructionsCommand, Unit>, IRequestHandler<CreateRecipeIngredientsCommand, Unit>
    {
        private readonly IRecipeRepository recipeRepository;
        private readonly IComponentRepository componentRepository;
        private readonly IDirectionRepository directionRepository;
        private readonly IIngredientsRepository ingredientsRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;
        private long recipeId;

        public CreateUserRecipeCommandHandler(IRecipeRepository recipeRepository,
                                              IComponentRepository componentRepository,
                                              IDirectionRepository directionRepository,
                                              IIngredientsRepository ingredientsRepository,
                                              ICurrentUserService currentUserService,
                                              IUnitOfWork unitOfWork)
        {
            this.recipeRepository = recipeRepository;
            this.componentRepository = componentRepository;
            this.directionRepository = directionRepository;
            this.ingredientsRepository = ingredientsRepository;
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;
        }

        public long RecipeId { get => recipeId; set => recipeId = value; }

        public async Task<Unit> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
        {
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
                Servings= request.Recipe.Servings,
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
            long userId = currentUserService.GetUser().UserId;

            byte componentStepNo = 0;
            byte instructionStepNo = 0;

            foreach(UserRecipeInstruction userRecipeInstruction  in request.RecipeInstructions)
            {
                componentStepNo++;
                long componentId =  await CreateRecipeComponent(componentStepNo, userRecipeInstruction.Component, userId);
                List<string> recipeInstructions = userRecipeInstruction.Directions
                                                 .Split('$', StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach(string recipeInstruction in recipeInstructions)
                {
                    instructionStepNo++;
                    await CreateRecipeInstruction(componentId,instructionStepNo, recipeInstruction, userId);
                }
            }
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }

        private async Task<long> CreateRecipeComponent(byte stepNo, string componentName, long userId)
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
            await componentRepository.AddAsync(recipeComponent);
            await unitOfWork.SaveAsync();

            return recipeComponent.ComponentId;
        }

        private async Task CreateRecipeInstruction(long componentId, byte stepNo, string instruction, long userId)
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
            await directionRepository.AddAsync(recipeInstruction);
        }
    }
}