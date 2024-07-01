using CookBook.Application.Common.Encryptor;
using CookBook.Application.Common.Models;
using CookBook.Application.Exceptions;
using CookBook.Application.Interface.Persistence;
using CookBook.Application.Interface.Persistence.Users;
using CookBook.Domain.Users;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
                    .MaximumLength(25).WithMessage("Your password length must not exceed 25")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\.\@\#]+").WithMessage("Your password must contain at least one (!? *.@).");
        }
    } 
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, long>
    {
        private readonly IValidator<CreateUserCommand> validator;
        private readonly IUserMasterRepository userMasterRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICryptor cryptor;

        public CreateUserCommandHandler(IValidator<CreateUserCommand> validator,
                                        IUserMasterRepository userMasterRepository,
                                        IUnitOfWork unitOfWork,
                                        ICryptor cryptor)
        {
            this.validator = validator;
            this.userMasterRepository = userMasterRepository;
            this.unitOfWork = unitOfWork;
            this.cryptor = cryptor;
        }
        public async Task<long> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            bool userExists = await userMasterRepository.GetAllAsNoTracking()
                            .AnyAsync(x => x.Email == request.User.Email);

            if (userExists)
                throw new BadRequestException("BE010001");

            string encryptedPassword = cryptor.Encrypt(request.User.Password);

            var user = new UserMaster()
            {
                FirstName = request.User.FirstName,
                LastName = request.User.LastName,
                FullName = request.User.LastName == null ? request.User.FirstName : request.User.FirstName + " " + request.User.LastName,
                Email = request.User.Email.Trim(),
                Password = encryptedPassword,
                CreatedOn = DateTime.Now,
                Status = 1,

            };
            await userMasterRepository.AddAsync(user);

            await unitOfWork.SaveAsync();

            return user.UserId;
        }
    }
}
