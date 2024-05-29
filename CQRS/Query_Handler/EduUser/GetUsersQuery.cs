using Edu_Block.DAL.EF;
using MediatR;
using Edu_Block_dev.CQRS.Query.EduUser;
using EduBlock.Model.DTO;
using System.Linq.Expressions;
using Edu_Block_dev.Helpers;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;

        public GetUsersQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<object>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var pageSize = request.PaginationUniversityUserDTO.PageSize;
            var pageNumber = request.PaginationUniversityUserDTO.Page;

            try
            {
                var count = _context.User.Where( u => u.IsDeleted == false ).Count();
                var query = _context.User.Join(_context.DepartmentStudents, u => u.Id, ds => ds.StudentId, (u, ds) => new
                {
                    User = u,
                    DepartmentStudents = ds
                }).Join(_context.Departments, ud => ud.DepartmentStudents.DepartmentId, d => d.Id, (uds, d) => new
                {
                    User = uds.User,
                    DepartmentStudents = uds.DepartmentStudents,
                    Department = d
                }).Join(_context.Universities, ud => ud.DepartmentStudents.UniversityId, u => u.Id, (udsd, u) => new
                {
                    User = udsd.User,
                    DepartmentStudents = udsd.DepartmentStudents,
                    Department = udsd.Department,
                    University = u
                })
                .Join(_context.UserProfiles,
                    udsdu => udsdu.User.Id,
                    profile => profile.UserID,
                    (udsdu, profile) => new
                    {
                        User = udsdu.User,
                        University = udsdu.University,
                        Department = udsdu.Department,
                        UserProfile = profile
                    })
                .GroupJoin(_context.Certificates,
                    udup => udup.UserProfile.Id,
                    certificate => certificate.UserProfileId,
                    (udup, certificates) => new
                    {
                        User = udup.User,
                        University = udup.University,
                        Department = udup.Department,
                        UserProfile = udup.UserProfile,
                        Certificates = certificates.ToList()
                    })
                .Join(_context.DockIoDIDs,
                    udup => udup.UserProfile.Id,
                    dock => dock.UserProfileId,
                    (_udup, dock) => new StudentDetailsDTO
                    {
                        User = _udup.User,
                        UserProfile = _udup.UserProfile,
                        University = _udup.University,
                        Department = _udup.Department,
                        Dock = dock,
                        Certificates = _udup.Certificates.OrderByDescending(c => c.CreatedAt).ToList()
                    });
                var filteredQuery = query;
                if (request.PaginationUniversityUserDTO.StartYear != 0 && request.PaginationUniversityUserDTO.EndYear != 0)
                {
                    filteredQuery = filteredQuery.Where(student =>
                        student.Certificates.Any(certificate =>
                            certificate.StartDate.Year >= request.PaginationUniversityUserDTO.StartYear &&
                            certificate.EndDate.Year <= request.PaginationUniversityUserDTO.EndYear)
                    );
                }
                var result = await Task.FromResult(filteredQuery);
                //IEnumerable<StudentDetailsDTO> enuUserProfileDTOs = result.ToList();
                if (request.PaginationUniversityUserDTO.Guid != Guid.Empty)
                {
                    result = result.Where(up => up.User.Id == request.PaginationUniversityUserDTO.Guid);
                }
                if (request.PaginationUniversityUserDTO.DepartmentId != Guid.Empty)
                {
                    result = result.Where(up => up.Department.Id == request.PaginationUniversityUserDTO.DepartmentId);
                }
                if (request.PaginationUniversityUserDTO.UniversityId != Guid.Empty)
                {
                    result = result.Where(up => up.University.Id == request.PaginationUniversityUserDTO.UniversityId);
                }
                IQueryable<StudentDetailsDTO> userProfileDTOs = result.AsQueryable<StudentDetailsDTO>();
                if (!string.IsNullOrWhiteSpace(request.PaginationUniversityUserDTO.SearchTerm))
                {
                    userProfileDTOs = userProfileDTOs.Where(p =>
                     p.User.Name.ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()) ||
                     ((string)p.User.Email).ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()));
                }
                if (request.PaginationUniversityUserDTO.SortOrder?.ToLower() == "desc")
                {
                    userProfileDTOs = userProfileDTOs.OrderByDescending(GetSortProperty(request));
                }
                else
                {
                    userProfileDTOs = userProfileDTOs.OrderBy(GetSortProperty(request));
                }
                var userProfileDTOList = await PagedList<StudentDetailsDTO>.CreateAsync(
                      userProfileDTOs,
                      request.PaginationUniversityUserDTO.Page,
                      request.PaginationUniversityUserDTO.PageSize);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: userProfileDTOList, message: "Users list");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new List<StudentDetailsDTO>(), message: ex.Message);
            }
        }

        private static Expression<Func<StudentDetailsDTO, object>> GetSortProperty(GetUsersQuery request)
        {
            return request.PaginationUniversityUserDTO.SortColumn?.ToLower() switch
            {
                "name" => u => u.User.Name,
                "email" => u => u.User.Email,
                _ => u => u.User.Id
            };
        }
    }
}