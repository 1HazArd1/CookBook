using iMocha.Talent.Analytics.Application.Interface.Persistence.Users;
using iMocha.Talent.Analytics.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iMocha.Talent.Analytics.Application.Users
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
