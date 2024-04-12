using iMocha.Talent.Analytics.Application.Users;
using iMocha.Talent.Analytics.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace iMocha.Talent.Analytics.Services.Controllers
{
    public class UserController : ApiControllerBase
    {
        private readonly IMediator mediator;
        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{CustomerId}")]
        public async Task<List<UserMaster>> Login( long CustomerId)
        {
            return await mediator.Send(new GetUsersByCustomerIdQuery(CustomerId));
        }
    }
}
