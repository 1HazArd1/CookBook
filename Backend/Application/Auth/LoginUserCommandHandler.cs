using CookBook.Application.Common.Encryptor;
using CookBook.Application.Exceptions;
using CookBook.Application.Interface.Persistence.Users;
using CookBook.Domain.Users;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Application.Auth
{
    public record LoginUserCommand(string Email, string Password) : IRequest<AuthSession>;

    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).NotEmpty().NotNull();
        }
    }
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthSession>
    {
        private readonly IAuthService authService;
        private readonly IUserMasterRepository userMasterRepository;
        private readonly IValidator<LoginUserCommand> validator;
        private readonly ICryptor cryptor;

        public LoginUserCommandHandler(IAuthService authService,
                                       IUserMasterRepository userMasterRepository,
                                       IValidator<LoginUserCommand> validator,
                                       ICryptor cryptor)
        {
            this.authService = authService;
            this.userMasterRepository = userMasterRepository;
            this.validator = validator;
            this.cryptor = cryptor;
        }
        public async Task<AuthSession> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            UserMaster? user = await userMasterRepository.GetAllAsNoTracking()
                               .Where(x => x.Email == request.Email.Trim())
                               .SingleOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new BadRequestException("BE010101");

            bool isPasswordVerified = cryptor.VerifyEncrypt(user.Password, request.Password);

            if (!isPasswordVerified)
                throw new UnauthorizedAccessException("BE010102");

            AuthSession authSession = authService.GetAuthSession(user);

            return authSession;
        }
    }
}
