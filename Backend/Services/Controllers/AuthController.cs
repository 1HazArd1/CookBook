using CookBook.Application.Auth;
using CookBook.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CookBook.Services.Controllers
{
    public class AuthController : ApiControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<long> Register([FromBody] User User)
        {
            return await mediator.Send(new CreateUserCommand(User));
        }

        [HttpPost("login")]
        public async Task<AuthSession> Login(LoginUserCommand loginUserCommand)
        {
            return await mediator.Send(loginUserCommand);
        }

    }
}
