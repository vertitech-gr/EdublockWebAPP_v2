using Edu_Block.DAL;
using MediatR;
using EduBlock.Model.DTO;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduAdmin;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetUniversityUserByIdQueryHandler : IRequestHandler<GetUniversityUserByUniqueIdQuery, UserWithProfile<UniversityUser>>
    {
        private readonly IRepository<UniversityUser> _universityUserRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<DockIoDID> _dockRepository;

        public GetUniversityUserByIdQueryHandler(IRepository<UniversityUser> universityUserRepository,IRepository<UserProfile> userProfileRepository, IRepository<Admin> adminRepository, IRepository<DockIoDID> dockRepository)
        {
            _universityUserRepository = universityUserRepository;
            _userProfileRepository = userProfileRepository;
            _dockRepository = dockRepository;
        }

        public async Task<UserWithProfile<UniversityUser>> Handle(GetUniversityUserByUniqueIdQuery request, CancellationToken cancellationToken)
        {
            UniversityUser universityUser = await _universityUserRepository.FindAsync(u => u.Id == request._uniqueId);
            UserProfile profile = await _userProfileRepository.FindAsync(u => u.UserID == request._uniqueId);
            DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == profile.Id);
            return new UserWithProfile<UniversityUser>(universityUser, profile, dock);
        }
    }
}
