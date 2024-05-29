using AutoMapper;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduUser;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetUserRoleByUserIdQueryHandler : IRequestHandler<GetUserRoleQuery, UserRole>
    {
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IMapper _mapper;

        public GetUserRoleByUserIdQueryHandler(IRepository<UserRole> userRoleRepository, IMapper mapper)
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }

        public async Task<UserRole> Handle(GetUserRoleQuery request, CancellationToken cancellationToken)
        {
            var output = await _userRoleRepository.FindAsync(u => u.UserId == request._userId);
            return output;
        }
    }
}
