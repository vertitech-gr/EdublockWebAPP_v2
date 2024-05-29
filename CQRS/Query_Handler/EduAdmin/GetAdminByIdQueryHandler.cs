
using Edu_Block.DAL;
using MediatR;
using EduBlock.Model.DTO;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduAdmin;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetAdminByIdQueryHandler : IRequestHandler<GetAdminByUniqueIdQuery, UserWithProfile<Admin>>
    {
        private readonly IRepository<Admin> _adminRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<DockIoDID> _dockRepository;

        public GetAdminByIdQueryHandler(IRepository<UserProfile> userProfileRepository, IRepository<Admin> adminRepository, IRepository<DockIoDID> dockRepository)
        {
            _adminRepository = adminRepository;
            _userProfileRepository = userProfileRepository;
            _dockRepository = dockRepository;
        }

        public async Task<UserWithProfile<Admin>> Handle(GetAdminByUniqueIdQuery request, CancellationToken cancellationToken)
        {
            Admin admin = await _adminRepository.FindAsync(u => u.Id == request._uniqueId);
            UserProfile profile = await _userProfileRepository.FindAsync(u => u.UserID == request._uniqueId);
            DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == profile.Id);
            return new UserWithProfile<Admin>(admin, profile, dock);
        }
    }
}
