using MediatR;

namespace CookBook.Services.Controllers
{
    public class UserController : ApiControllerBase
    {
        private readonly IMediator mediator;
        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        //[HttpGet("{CustomerId}")]
        //public async Task<List<UserMaster>> Login( long CustomerId)
        //{
        //    return await mediator.Send(new GetUsersByCustomerIdQuery(CustomerId));
        //}
    }
}
