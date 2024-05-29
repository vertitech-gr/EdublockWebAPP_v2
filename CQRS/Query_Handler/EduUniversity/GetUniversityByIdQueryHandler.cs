using Edu_Block.DAL;
using MediatR;
using EduBlock.Model.DTO;
using Edu_Block_dev.CQRS.Query.EduUniversity;
using Edu_Block_dev.Modal.DTO;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetUniversityByIdQueryHandler : IRequestHandler<GetUniversityByUniqueIdQuery, UserWithProfile<University>>
    {
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<UniversityDetail> _universityDetailRepository;
        private readonly IRepository<DockIoDID> _dockRepository;

        public GetUniversityByIdQueryHandler(IRepository<UserProfile> userProfileRepository, IRepository<University> universityRepository, IRepository<DockIoDID> dockRepository, IRepository<UniversityDetail> universityDetailRepository)
        {
            _universityRepository = universityRepository;
            _userProfileRepository = userProfileRepository;
            _universityDetailRepository = universityDetailRepository;
            _dockRepository = dockRepository;
        }

        public async Task<UserWithProfile<University>> Handle(GetUniversityByUniqueIdQuery request, CancellationToken cancellationToken)
        {
            University university = await _universityRepository.FindAsync(u => u.Id == request._uniqueId);
            UserProfile profile = await _userProfileRepository.FindAsync(u => u.UserID == request._uniqueId);
            UniversityDetail universityDetail = await _universityDetailRepository.FindAsync(u => u.UniversityId == request._uniqueId);
            university.UniversityDetail = universityDetail;
            DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == profile.Id);
            return new UserWithProfile<University>(university, profile, dock);
        }
    }
}
