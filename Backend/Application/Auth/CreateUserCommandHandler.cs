using CookBook.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace CookBook.Application.Auth
{
    public record CreateUserCommand(User User) : IRequest<long>;

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator() 
        {
            RuleFor(x => x.User.FirstName).NotEmpty();
            RuleFor(x => x.User.Email)
                    .Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$").WithMessage("Invalid Email")
                    .NotEmpty();
            RuleFor(x => x.User.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        }
    } 
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, long>
    {
        private readonly IValidator<CreateUserCommand> validator;

        public CreateUserCommandHandler(IValidator<CreateUserCommand> validator)
        {
            this.validator = validator;
        }
        public async Task<long> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            throw new NotImplementedException();
        }
    }
}
