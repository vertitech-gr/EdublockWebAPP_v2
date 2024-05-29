using AutoMapper;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using MediatR;
using Edu_Block_dev.CQRS.Query.EduUser;
using EduBlock.Model.DTO;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetUserDetailsByIdQueryHandler : IRequestHandler<GetUserByUniqueIdQuery, UserWithProfile<User>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<DockIoDID> _dockRepository;
        private readonly IMapper _mapper;

        public GetUserDetailsByIdQueryHandler(IRepository<UserProfile> userProfileRepository, IRepository<User> userRepository, IRepository<DockIoDID> dockRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _dockRepository = dockRepository;
            _mapper = mapper;
        }

        public async Task<UserWithProfile<User>> Handle(GetUserByUniqueIdQuery request, CancellationToken cancellationToken)
        {
            User employer = await _userRepository.FindAsync(u => u.Id == request._uniqueId);
            UserProfile profile = await _userProfileRepository.FindAsync(u => u.UserID == request._uniqueId);
            DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == profile.Id);
            return new UserWithProfile<User>(employer, profile, dock);
        }
    }
}
