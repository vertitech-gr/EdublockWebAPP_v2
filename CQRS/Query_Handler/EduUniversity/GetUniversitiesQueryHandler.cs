using System.Linq.Expressions;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduUniversity;
using Edu_Block_dev.Helpers;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUniversity
{
    public class GetUniversitiesQueryHandler : IRequestHandler<GetUniversitiesQuery, ApiResponse<object>>
    {
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<UserProfile> _profileRepository;
        private readonly IRepository<DockIoDID> _dockRepository;
        private readonly IRepository<DepartmentStudent> _departmentStudentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UniversityDetail> _universityDetailsRepository;
        private readonly EduBlockDataContext _context;

        public GetUniversitiesQueryHandler(EduBlockDataContext context,
            IRepository<University> universityRepository,
            IRepository<UserProfile> profileRepository,
            IRepository<DockIoDID> dockRepository,
            IRepository<User> userRepository,
            IRepository<DepartmentStudent> departmentStudentRepository,
            IRepository<UniversityDetail> universityDetailsRepository)
        {
            _context = context;
            _universityRepository = universityRepository;
            _profileRepository = profileRepository;
            _dockRepository = dockRepository;
            _userRepository = userRepository;
            _universityDetailsRepository = universityDetailsRepository;
            _departmentStudentRepository = departmentStudentRepository; 
        }

        public async Task<ApiResponse<object>> Handle(GetUniversitiesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<University> enuUniversities = await _universityRepository.GetAllAsync();
            IEnumerable<UserProfile> enuProfiles = await _profileRepository.GetAllAsync();
            IEnumerable<DockIoDID> enuDocks = await _dockRepository.GetAllAsync();
            IEnumerable<DepartmentStudent> enuDepartmentStudents = await _departmentStudentRepository.GetAllAsync();
            IEnumerable<User> enuUser = await _userRepository.GetAllAsync();

            IEnumerable<User> users = enuUser.Where( eu => eu.IsDeleted == false ).AsQueryable();
            IQueryable<University> universities = enuUniversities.AsQueryable();
            IQueryable<UserProfile> profiles = enuProfiles.AsQueryable();
            IQueryable<DockIoDID> docks = enuDocks.AsQueryable();
            IQueryable<DepartmentStudent> departmentStudents = enuDepartmentStudents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.UniversityPaginationDTO.SearchTerm))
            {
                universities = universities.Where(p =>
                    p.Name.ToLower().Contains(request.UniversityPaginationDTO.SearchTerm.ToLower()) ||
                    ((string)p.Email).ToLower().Contains(request.UniversityPaginationDTO.SearchTerm.ToLower()) ||
                    ((string)p.Address).ToLower().Contains(request.UniversityPaginationDTO.SearchTerm.ToLower()) ||
                    ( "+"+p.CountryCode.ToString()+"-"+p.PhoneNumber.Replace(p.CountryCode, "").ToString()).ToLower().Contains(request.UniversityPaginationDTO.SearchTerm.ToLower()) ||
                    (p.Status.ToString()).ToLower().Contains(request.UniversityPaginationDTO.SearchTerm.ToLower())
                    );
            }

            var query = from university in universities
                        join departmentStudent in departmentStudents
                            on university.Id equals departmentStudent.UniversityId into departmentStudentGroup
                        from departmentStudent in departmentStudentGroup.DefaultIfEmpty()
                        join user in users
                            on (departmentStudent != null ? departmentStudent.StudentId : (Guid?)null) equals user.Id into userGroup
                        from user in userGroup.DefaultIfEmpty()
                        join profile in profiles
                            on university.Id equals profile.UserID into profileGroup
                        from profile in profileGroup.DefaultIfEmpty()
                        join dock in docks
                            on profile != null ? profile.Id : (Guid?)null equals dock.UserProfileId into dockGroup
                        from dock in dockGroup.DefaultIfEmpty()
                        select new
                        {
                            UniversityId = university.Id,
                            UniversityName = university.Name,
                            UniversityAddress = university.Address,
                            UniversityEmail = university.Email,
                            UniversityCountryCode = university.CountryCode,
                            UniversityPhoneNumber = university.PhoneNumber.Replace(university.CountryCode, ""),
                            UniversityType = university.Type,
                            UniversityActive = university.Active,
                            UniversityStatus = university.Status,
                            UniversityLoginStatus = university.loginStatus,
                            UniversityInviteDate = university.CreatedAt,
                            ProfileId = profile != null ? profile.Id : Guid.Empty,
                            ProfileName = profile != null ? profile.Id : Guid.Empty,
                            DockId = dock != null ? dock.Id : Guid.Empty,
                            Did = dock != null ? dock.DID : string.Empty,
                            UserId = user != null ? user.Id : Guid.Empty,
                        };

            IQueryable<UniversityResponseDTO> universityResponseQuery = query.Select(u => new UniversityResponseDTO
            {
                Id = u.UniversityId,
                Name = u.UniversityName,
                Address = u.UniversityAddress,
                Email = u.UniversityEmail,
                CountryCode = u.UniversityCountryCode,
                PhoneNumber = u.UniversityPhoneNumber,
                Type = u.UniversityType,
                Active = u.UniversityActive,
                Status = u.UniversityStatus,
                LoginStatus = u.UniversityLoginStatus,
                InviteDate = u.UniversityInviteDate,
                UserProfileId = u.ProfileId,
                did = u.Did,
                UserId = u.UserId   
            }).GroupBy(u => u.Id)         
            .Select(g => g.First());

            if (request.UniversityPaginationDTO.StudentId != Guid.Empty)
            {
                universityResponseQuery = universityResponseQuery.Where(urq => urq.UserProfileId == request.UniversityPaginationDTO.StudentId);
            }

            if (request.UniversityPaginationDTO.SortOrder?.ToLower() == "desc")
            {
                universityResponseQuery = universityResponseQuery.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                universityResponseQuery = universityResponseQuery.OrderBy(GetSortProperty(request));
            }

            var universityResponseList = await PagedList<UniversityResponseDTO>.CreateAsync(
                universityResponseQuery,
                request.UniversityPaginationDTO.Page,
                request.UniversityPaginationDTO.PageSize);

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: universityResponseList, message: "University list");
        }

        private static Expression<Func<UniversityResponseDTO, object>> GetSortProperty(GetUniversitiesQuery request)
        {
            return request.UniversityPaginationDTO.SortColumn?.ToLower() switch
            {
                "name" => university => university.Name,
                "address" => university => university.Address,
                "email" => university => university.Email,
                "phoneNumber" => university => university.PhoneNumber,
                "status" => university => university.Status,
                "InviteDate" => university => university.InviteDate,
                _ => university => university.InviteDate
            };
        }
    }

    
}