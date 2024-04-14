using CookBook.Application.Interface.Persistence.Users;
using CookBook.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Application.Users
{
    public record GetUsersByCustomerIdQuery(long CustomerId) : IRequest<List<UserMaster>>;
    public class GetUsersByCustomerIdQueryHandler : IRequestHandler<GetUsersByCustomerIdQuery, List<UserMaster>>
    {
        private readonly IUserMasterRepository userMasterRepository;
        public GetUsersByCustomerIdQueryHandler(IUserMasterRepository userMasterRepository)
        {
                this.userMasterRepository = userMasterRepository;
        }

        public async Task<List<UserMaster>> Handle(GetUsersByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            var users=await userMasterRepository.GetAllAsNoTracking()
                      .Where(x=>x.CustomerId == request.CustomerId).ToListAsync();

            return users;
        }
    }
}
