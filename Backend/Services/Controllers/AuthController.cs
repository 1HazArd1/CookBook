using CookBook.Application.Auth;
using CookBook.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CookBook.Services.Controllers
{
    public class AuthController : ApiAuthorizeControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task<long> Register([FromQuery] User User)
        {
            return await mediator.Send(new CreateUserCommand(User));
        }
    }
}
