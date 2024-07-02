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
    public record CreateRecipeInstructionCommand(List<RecipeInstruction> RecipeInstructions) : IRequest<Unit>;
    public class CreateUserRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Unit>, IRequestHandler<CreateRecipeInstructionCommand, Unit>
    {
        private readonly IRecipeRepository recipeRepository;
        private readonly IComponentRepository componentRepository;
        private readonly IDirectionRepository directionRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;
        private LoggedInUser loggedInUser;
        private long recipeId;

        public CreateUserRecipeCommandHandler(IRecipeRepository recipeRepository,
                                              IComponentRepository componentRepository,
                                              IDirectionRepository directionRepository,
                                              ICurrentUserService currentUserService,
                                              IUnitOfWork unitOfWork)
        {
            this.recipeRepository = recipeRepository;
            this.componentRepository = componentRepository;
            this.directionRepository = directionRepository;
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;
        }

        public long RecipeId { get => recipeId; set => recipeId = value; }

        public async Task<Unit> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
        {
            loggedInUser = currentUserService.GetUser();

            Domain.Dishes.Recipe? existingRecipe = await recipeRepository.GetAllAsNoTracking()
                                                  .Where(x => x.UserId == loggedInUser.UserId && x.RecipeName.ToLower() == request.Recipe.Name.Trim().ToLower())
                                                  .FirstOrDefaultAsync(cancellationToken);

            if (existingRecipe != null)
                throw new BadRequestException("BE020101");

            Domain.Dishes.Recipe userRecipe = new()
            {
                RecipeName = request.Recipe.Name,
                Cuisine = request.Recipe.Cuisine,
                RecipeUrl = request.Recipe.RecipeUrl,
                UserId = loggedInUser.UserId,
                Duration = request.Recipe.Duration,
                Status = 1,
                CreatedBy = loggedInUser.UserId,
                CreatedOn = DateTime.Now,
                ModifiedBy = loggedInUser.UserId,
                ModifiedOn = DateTime.Now
            };

            await recipeRepository.AddAsync(userRecipe);
            await unitOfWork.SaveAsync();

            recipeId = userRecipe.RecipeId;

            return Unit.Value;
        }

        public async Task<Unit> Handle(CreateRecipeInstructionCommand request, CancellationToken cancellationToken)
        {
            byte componentStepNo = 0;
            byte instructionStepNo = 0;
            foreach(RecipeInstruction recipeComponent  in request.RecipeInstructions)
            {
                componentStepNo++;
                long componentId =  await CreateRecipeComponent(componentStepNo, recipeComponent.ComponentName);
                foreach(string recipeInstruction in recipeComponent.Directions)
                {
                    instructionStepNo++;
                    await CreateRecipeInstruction(componentId,instructionStepNo, recipeInstruction);
                }
            }
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }

        private async Task<long> CreateRecipeComponent(byte stepNo, string componentName)
        {
            Component recipeComponent = new()
            {
                UserId = loggedInUser.UserId,
                RecipeId = recipeId,
                StepNo = stepNo,
                ComponentName = componentName,
                Status = 1,
                CreatedBy = loggedInUser.UserId,
                CreatedOn = DateTime.Now,
                ModifiedBy = loggedInUser.UserId,
                ModifiedOn = DateTime.Now
            };
            await componentRepository.AddAsync(recipeComponent);
            await unitOfWork.SaveAsync();
            return recipeComponent.ComponentId;
        }



        private async Task CreateRecipeInstruction(long componentId, byte stepNo, string Instruction)
        {
            Direction recipeInstruction = new()
            {
                UserId = loggedInUser.UserId,
                ComponentId = componentId,
                StepNo = stepNo,
                Instruction = Instruction,
                Status = 1,
                CreatedBy = loggedInUser.UserId,
                CreatedOn = DateTime.Now,
                ModifiedBy = loggedInUser.UserId,
                ModifiedOn = DateTime.Now
            };
            await directionRepository.AddAsync(recipeInstruction);
        }
    }
}