using CookBook.Application.Common.Models;
using CookBook.Application.Interface.Auth;
using CookBook.Application.Interface.Persistence;
using CookBook.Application.Interface.Persistence.Dishes;
using MediatR;

namespace CookBook.Application.Dishes.Commands
{
    public record CreateRecipeCommand(Common.Models.Recipe Recipe) : IRequest<Unit>;
    public record CreateDirectionCommand(List<RecipeInstruction> RecipeInstructions) : IRequest<Unit>;
    public class CreateUserRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Unit>, IRequestHandler<CreateDirectionCommand, Unit>
    {
        private readonly IRecipeRepository recipeRepository;
        private readonly IComponentRepository componentRepository;
        private readonly IDirectionRepository directionRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;
        private LoggedInUser loggedInUser;

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
        public async Task<Unit> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
        {
            loggedInUser = currentUserService.GetUser();

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

            return Unit.Value;
        }

        public Task<Unit> Handle(CreateDirectionCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}