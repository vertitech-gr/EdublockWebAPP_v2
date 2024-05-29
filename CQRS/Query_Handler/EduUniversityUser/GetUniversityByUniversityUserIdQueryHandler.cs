using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduAdmin;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUniversityUser
{
    public class GetUniversityByUniversityUserIdQueryHandler : IRequestHandler<GetUniversityByUniversityUserIdQuery, UniversityResponseDTO>
    {
        private readonly EduBlockDataContext _dbContext;

        public GetUniversityByUniversityUserIdQueryHandler(EduBlockDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<UniversityResponseDTO> Handle(GetUniversityByUniversityUserIdQuery request, CancellationToken cancellationToken)
        {
            var university = from uu in _dbContext.UniversityUsers
                             join udu in _dbContext.UniversityDepartmentUsers on uu.Id equals udu.UniversityUserId into universityDepartmentUsers
                             from udu in universityDepartmentUsers.DefaultIfEmpty()
                             join u in _dbContext.Universities on udu.UniversityId equals u.Id into universityUser
                             from u in universityUser.DefaultIfEmpty()
                             join up in _dbContext.UserProfiles on u.Id equals up.UserID into userProfile
                             from up in userProfile.DefaultIfEmpty()
                             join d in _dbContext.DockIoDIDs on up.Id equals d.UserProfileId into dock
                             from d in dock.DefaultIfEmpty()
                             where uu.Id == request.uniqueId
                             select new UniversityResponseDTO
                             {
                                 Id = u.Id,
                                 Name = u.Name,
                                 Email = u.Email,
                                 Address = u.Address,
                                 CountryCode = u.CountryCode,
                                 PhoneNumber = u.PhoneNumber,
                                 did = d.DID.ToString()
                             };
            UniversityResponseDTO universityResponseDTO = university.FirstOrDefault();
            return universityResponseDTO;
        }
    }
}

