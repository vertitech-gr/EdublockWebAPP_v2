using Edu_Block.DAL.EF;
using MediatR;
using Edu_Block_dev.CQRS.Query.EduUser;
using EduBlock.Model.DTO;
using System.Linq.Expressions;
using Edu_Block.DAL;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetUniversityUsersQueryHandler : IRequestHandler<GetUniversityUsersQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<DockIoDID> _dockIoDIDRepository;
        private readonly IRepository<Certificate> _certificateRepository;

        public GetUniversityUsersQueryHandler(EduBlockDataContext context, IRepository<User> userRepository, IRepository<UserProfile> userProfileRepository, IRepository<DockIoDID> dockIoDIDRepository, IRepository<Certificate> certificateRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _dockIoDIDRepository = dockIoDIDRepository;
            _certificateRepository = certificateRepository;
        }

        public async Task<ApiResponse<object>> Handle(GetUniversityUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var universityUsers = _context.DepartmentStudents
                   .Where(ds =>
                   (request.PaginationUniversityUserDTO.UniversityId == Guid.Empty || ds.UniversityId == request.PaginationUniversityUserDTO.UniversityId) &&
                   (request.PaginationUniversityUserDTO.DepartmentId == Guid.Empty || ds.DepartmentId == request.PaginationUniversityUserDTO.DepartmentId) &&
                   (request.PaginationUniversityUserDTO.Guid == Guid.Empty || ds.StudentId == request.PaginationUniversityUserDTO.Guid)
                   )
                   .Join(_context.UserProfiles, ds => ds.StudentId, up => up.UserID, (ds, up) => new
                   {
                       UserProfile = up,
                       DepartmentStudent = ds
                   })
                   .Join(_context.User, upds => upds.DepartmentStudent.StudentId, u => u.Id, (upds, u) => new
                   {
                       UserProfile = upds.UserProfile,
                       DepartmentStudent = upds.DepartmentStudent,
                       User = u
                   })
                   .Join(_context.DockIoDIDs, updsu => updsu.UserProfile.Id, d => d.UserProfileId, (updsu, d) => new
                   {
                       UserProfile = updsu.UserProfile,
                       DepartmentStudent = updsu.DepartmentStudent,
                       User = updsu.User,
                       Dock = d
                   })
                   .GroupJoin(_context.Certificates, updsud => updsud.UserProfile.Id, c => c.UserProfileId, (updsud, c) => new
                   {
                       User = updsud.User,
                       UserProfile = updsud.UserProfile,
                       DepartmentStudent = updsud.DepartmentStudent,
                       Dock = updsud.Dock,
                       Certificates = c.ToList()
                   }).Select( u => new StudentDetailsDTO {
                       User = u.User,
                       UserProfile = u.UserProfile,
                       Dock = u.Dock,
                       Certificates = u.Certificates
                   } );


                universityUsers = universityUsers.Where( uu => uu.User.IsDeleted == false);

                if (request.PaginationUniversityUserDTO.StartYear != 0 && request.PaginationUniversityUserDTO.EndYear != 0)
                {
                    universityUsers = universityUsers.Where(student =>
                        student.Certificates.Any(certificate =>
                            certificate.StartDate.Year >= request.PaginationUniversityUserDTO.StartYear &&
                            certificate.EndDate.Year <= request.PaginationUniversityUserDTO.EndYear)
                    );
                }


                if (!string.IsNullOrWhiteSpace(request.PaginationUniversityUserDTO.SearchTerm))
                {
                    universityUsers = universityUsers.Where(p =>
                     p.User.Name.ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()) ||
                     ((string)p.User.Email).ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()));
                }

                var totalCount = universityUsers.Count();

              
                if (request.PaginationUniversityUserDTO.SortOrder?.ToLower() == "desc")
                {
                    universityUsers = universityUsers.OrderByDescending(GetSortProperty(request));
                }
                else
                {
                    universityUsers = universityUsers.OrderBy(GetSortProperty(request));
                }

                universityUsers = universityUsers.Skip((request.PaginationUniversityUserDTO.Page - 1) * request.PaginationUniversityUserDTO.PageSize).Take(request.PaginationUniversityUserDTO.PageSize);



                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { items= universityUsers, request.PaginationUniversityUserDTO.Page, request.PaginationUniversityUserDTO.PageSize, totalCount, hasNextPage = (request.PaginationUniversityUserDTO.Page * request.PaginationUniversityUserDTO.PageSize < totalCount), hasPreviousPage = (request.PaginationUniversityUserDTO.Page > 1) }, message: "Users list");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new List<StudentDetailsDTO>(), message: ex.Message);
            }
        }

        private Expression<Func<StudentDetailsDTO, object>> GetSortProperty(GetUniversityUsersQuery request)
        {
            switch (request.PaginationUniversityUserDTO.SortColumn?.ToLower())
            {
                case "name":
                    return u => u.User.Name;
                case "email":
                    return u => u.User.Email;
                default:
                    return u => u.User.Id;
            }
        }

    }
}