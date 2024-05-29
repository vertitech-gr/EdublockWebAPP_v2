using System.Linq.Expressions;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Helpers;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block_dev.CQRS.Query_Handler.EduCertificate
{
    public class CertificateResponseQueryHandler : IRequestHandler<GetCertificateResponseQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<Role>  _roleRepository;

        public CertificateResponseQueryHandler(EduBlockDataContext context, IRepository<Role> roleRepository)
        {
            _context = context;
            _roleRepository = roleRepository;
        }

        public async Task<ApiResponse<object>> Handle(GetCertificateResponseQuery request, CancellationToken cancellationToken)

        {

            _context.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
            IEnumerable<CertificateResponseDTO> enuCertificateResponseDTO = null;

            if (request.User != null && request.User.RolesAndPermissionDTO.Role.NormalizedName == "STUDENT")
            {
                //   enuCertificateResponseDTO = await _context.Certificates
                //   .Where(u => u.UserProfileId.Equals(request.User.UserProfile.Id))
                //.Join(_context.UserProfiles, c => c.UserProfileId, up => up.Id, (c, up) => new
                //{
                //    Certificate = c,
                //    UserProfile = up
                //})
                //.Join(_context.User, cup => cup.UserProfile.UserID, u => u.Id, (cup, u) => new
                //{
                //    Certificate = cup.Certificate,
                //    UserProfile = cup.UserProfile,
                //    User = u
                //})
                //.Join(_context.DepartmentStudents, cuu => cuu.UserProfile.UserID, ds => ds.StudentId, (cuu, ds) => new
                //{
                //    Certificate = cuu.Certificate,
                //    UserProfile = cuu.UserProfile,
                //    User = cuu.User,
                //    DepartmentStudents = ds
                //})
                //.Join(_context.Universities, cuud => cuud.DepartmentStudents.UniversityId, u => u.Id, (cuud, u) => new CertificateResponseDTO
                //{
                //    CertificateId = cuud.Certificate.Id,
                //    Name = u.Name,
                //    Email = cuud.User.Email,
                //    DegreeAwardedDate = cuud.Certificate.DegreeAwardedDate,
                //    MarksAndGrades = "6.5",
                //    CertificationType = cuud.Certificate.DegreeType,
                //    PricipalName = "George",
                //    OfficialId = cuud.User.RollNo,
                //    certifcateText = "This certificate is uploaded"
                //})
                //.ToListAsync();

                enuCertificateResponseDTO = await (
                    from certificate in _context.Certificates
                    where certificate.UserProfileId.Equals(request.User.UserProfile.Id)
                    join userProfile in _context.UserProfiles on certificate.UserProfileId equals userProfile.Id
                    join user in _context.User on userProfile.UserID equals user.Id
                    join departmentStudent in _context.DepartmentStudents on userProfile.UserID equals departmentStudent.StudentId
                    join university in _context.Universities on departmentStudent.UniversityId equals university.Id
                    select new CertificateResponseDTO
                    {
                        CertificateId = certificate.Id,
                        Name = university.Name,
                        Email = user.Email,
                        DegreeAwardedDate = certificate.DegreeAwardedDate,
                        MarksAndGrades = "6.5",
                        CertificationType = certificate.DegreeType,
                        PricipalName = user.Name,
                        OfficialId = user.RollNo.ToString(),
                        certifcateText = "This certificate is uploaded"
                    }
                    ).ToListAsync();
            }
            else
            {
                // enuCertificateResponseDTO = await _context.Certificates
                //.Join(_context.UserProfiles, c => c.UserProfileId, up => up.Id, (c, up) => new
                //{
                //    Certificate = c,
                //    UserProfile = up
                //})
                //.Join(_context.User, cup => cup.UserProfile.UserID, u => u.Id, (cup, u) => new
                //{
                //    Certificate = cup.Certificate,
                //    UserProfile = cup.UserProfile,
                //    User = u
                //})
                //.Join(_context.DepartmentStudents, cuu => cuu.UserProfile.UserID, ds => ds.StudentId, (cuu, ds) => new
                //{
                //    Certificate = cuu.Certificate,
                //    UserProfile = cuu.UserProfile,
                //    User = cuu.User,
                //    DepartmentStudents = ds
                //})
                //.Join(_context.Universities, cuud => cuud.DepartmentStudents.UniversityId, u => u.Id, (cuud, u) => new CertificateResponseDTO
                //{
                //    CertificateId = cuud.Certificate.Id,
                //    Name = u.Name,
                //    Email = cuud.User.Email,
                //    DegreeAwardedDate = cuud.Certificate.DegreeAwardedDate,
                //    MarksAndGrades = "6.5",
                //    CertificationType = cuud.Certificate.DegreeType,
                //    PricipalName = "George",
                //    OfficialId = cuud.User.RollNo,
                //    certifcateText = "This certificate is uploaded",
                //    DepartmentId = cuud.DepartmentStudents.DepartmentId,
                //    UniversityId = cuud.DepartmentStudents.UniversityId,
                //    StartDate = cuud.Certificate.StartDate,
                //    EndDate = cuud.Certificate.EndDate
                //})
                //.ToListAsync();

                enuCertificateResponseDTO = await (
        from certificate in _context.Certificates
        join userProfile in _context.UserProfiles on certificate.UserProfileId equals userProfile.Id
        join user in _context.User on userProfile.UserID equals user.Id
        join departmentStudent in _context.DepartmentStudents on userProfile.UserID equals departmentStudent.StudentId
        join university in _context.Universities on departmentStudent.UniversityId equals university.Id
        select new CertificateResponseDTO
        {
            CertificateId = certificate.Id,
            Name = university.Name,
            Email = user.Email,
            DegreeAwardedDate = certificate.DegreeAwardedDate,
            MarksAndGrades = "6.5",
            CertificationType = certificate.DegreeType,
            PricipalName = user.Name,
            OfficialId = user.RollNo.ToString(),
            certifcateText = "This certificate is uploaded",
            DepartmentId = departmentStudent.DepartmentId,
            UniversityId = departmentStudent.UniversityId,
            StartDate = certificate.StartDate,
            EndDate = certificate.EndDate
        }
    ).ToListAsync();

            }

            if (request.PaginationUniversityUserDTO.StartYear != 0 && request.PaginationUniversityUserDTO.StartYear > 0 ) 
            {
                enuCertificateResponseDTO = enuCertificateResponseDTO.Where(u => u.StartDate.Year >=  request.PaginationUniversityUserDTO.StartYear);
            }

            if (request.PaginationUniversityUserDTO.EndYear != 0 && request.PaginationUniversityUserDTO.EndYear > 0)
            {
                enuCertificateResponseDTO = enuCertificateResponseDTO.Where(u => u.EndDate.Year <= request.PaginationUniversityUserDTO.EndYear);
            }


            if (request.PaginationUniversityUserDTO.DepartmentId != Guid.Empty)
            {
                enuCertificateResponseDTO = enuCertificateResponseDTO.Where(u => u.DepartmentId == request.PaginationUniversityUserDTO.DepartmentId);
            }

            if (request.PaginationUniversityUserDTO.UniversityId != Guid.Empty)
            {
                enuCertificateResponseDTO = enuCertificateResponseDTO.Where(u => u.UniversityId == request.PaginationUniversityUserDTO.UniversityId);
            }

            if (request.PaginationUniversityUserDTO.Guid != Guid.Empty)
            {
                enuCertificateResponseDTO = enuCertificateResponseDTO.Where(u => u.CertificateId == request.PaginationUniversityUserDTO.Guid);
            }


            IQueryable<CertificateResponseDTO> certificateResponseDTOs = enuCertificateResponseDTO.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.PaginationUniversityUserDTO.SearchTerm))
            {
                certificateResponseDTOs = certificateResponseDTOs.Where(p =>
                    p.Name.ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()) ||
                     p.PricipalName.ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()) ||
                     p.CertificationType.ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()) ||
                    ((string)p.Email).ToLower().Contains(request.PaginationUniversityUserDTO.SearchTerm.ToLower()));
            }
            if (request.PaginationUniversityUserDTO.SortOrder?.ToLower() == "desc")
            {
                certificateResponseDTOs = certificateResponseDTOs.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                certificateResponseDTOs = certificateResponseDTOs.OrderBy(GetSortProperty(request));
            }

            var certificateList = await PagedList<CertificateResponseDTO>.CreateAsync(
                            certificateResponseDTOs,
                            request.PaginationUniversityUserDTO.Page,
            request.PaginationUniversityUserDTO.PageSize);


            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: certificateList, message: "certificate list");

        }

        private static Expression<Func<CertificateResponseDTO, object>> GetSortProperty(GetCertificateResponseQuery request)
        {
            return request.PaginationUniversityUserDTO.SortColumn?.ToLower() switch
            {
                "Name" => certificate => certificate.Name,
                "Email" => certificate => certificate.Email,
                "DegreeAwardedDate" => certificate => certificate.DegreeAwardedDate,
                _ => certificate => certificate.DegreeAwardedDate
            };
        }
    }
}