using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Query.EduUser;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class UserProfileByIdQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfile>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<UserProfile> _userRepository;

        public UserProfileByIdQueryHandler(EduBlockDataContext context, IRepository<UserProfile> userRepository)
        {
            _context = context;
            _userRepository = userRepository;

        }

        public async Task<UserProfile> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            if (request._userProfile == null)
            {
                return null;
            }
            return await _userRepository.FindAsync(u => u.Id == request._userProfile.Id);

        }
    }

}
