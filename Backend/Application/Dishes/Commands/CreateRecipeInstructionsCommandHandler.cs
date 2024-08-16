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
    public record CreateRecipeInstructionsCommand(List<UserRecipeInstruction> RecipeInstructions, long RecipeId) : IRequest<Unit>;

    public class CreateRecipeInstructionsCommandValidator : AbstractValidator<CreateRecipeInstructionsCommand>
    {
        public CreateRecipeInstructionsCommandValidator()
        {
            RuleFor(x => x.RecipeInstructions)
                    .Must(instructions => instructions.Select(y => y.Component.ToLower().Trim()).Distinct().Count() == instructions.Count)
                    .WithMessage("Duplicate component names are not allowed");
        }

    }
    public class CreateRecipeInstructionsCommandHandler : IRequestHandler<CreateRecipeInstructionsCommand, Unit>
    {
        private readonly IComponentRepository componentRepository;
        private readonly IDirectionRepository directionRepository;
        private readonly IRecipeRepository recipeRepository;
        private readonly IValidator<CreateRecipeInstructionsCommand> instructionsValidator;
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;

        public CreateRecipeInstructionsCommandHandler(IComponentRepository componentRepository,
                                                      IDirectionRepository directionRepository,
                                                      IRecipeRepository recipeRepository,
                                                      IValidator<CreateRecipeInstructionsCommand> instructionsValidator,
                                                      ICurrentUserService currentUserService,
                                                      IUnitOfWork unitOfWork)
        {
            this.componentRepository = componentRepository;
            this.directionRepository = directionRepository;
            this.recipeRepository = recipeRepository;
            this.instructionsValidator = instructionsValidator;
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(CreateRecipeInstructionsCommand request, CancellationToken cancellationToken)
        {
            await instructionsValidator.ValidateAndThrowAsync(request, cancellationToken);

            bool isRecipeValid = await recipeRepository.GetAllAsNoTracking()
                                .AnyAsync(x => x.RecipeId == request.RecipeId, cancellationToken);

            if (!isRecipeValid)
                throw new BadRequestException("BE020201");

            long userId = currentUserService.GetUser().UserId;

            byte componentStepNo = 0;
            byte instructionStepNo = 0;

            List<Component> components = new();
            List<Direction> directions = new();

            List<string> componentNames = request.RecipeInstructions.Select(x => x.Component).ToList();

            foreach (UserRecipeInstruction userRecipeInstruction in request.RecipeInstructions)
            {
                componentStepNo++;
                components.Add(CreateRecipeComponent(componentStepNo, userRecipeInstruction.Component, userId, request.RecipeId));
            }
            await componentRepository.AddRangeAsync(components);
            await unitOfWork.SaveAsync();

            Dictionary<string, long> componentMapping = await componentRepository.GetAllAsNoTracking()
                                                       .Where(x => x.UserId == userId && x.RecipeId == request.RecipeId && componentNames.Contains(x.ComponentName))
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

        private static Component CreateRecipeComponent(byte stepNo, string componentName, long userId, long recipeId)
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
