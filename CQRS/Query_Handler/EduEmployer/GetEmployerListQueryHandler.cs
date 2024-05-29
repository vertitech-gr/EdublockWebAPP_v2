using System.Linq.Expressions;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduEmployer;
using Edu_Block_dev.Helpers;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler
{
    public class GetEmployerListQueryHandler : IRequestHandler<GetEmployerListQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;

        public GetEmployerListQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<object>> Handle(GetEmployerListQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<EmployerResponseDTO> enuEmployerResponseQuery = _context.Employers.Join(_context.UserProfiles,
                                   er => er.Id,
                                   up => up.UserID,
                                   (er, up) => new
                                   {
                                       Employer = er,
                                       UserProfile = up
                                   }).Join(_context.DockIoDIDs,
                                   er => er.UserProfile.Id,
                                   dock => dock.UserProfileId,
                                   (er, dock) => new EmployerResponseDTO
                                   {
                                       Id = er.Employer.Id,
                                       Name = er.Employer.Name,
                                       Email = er.Employer.Email,
                                       Address = er.Employer.Address,
                                       Industry = er.Employer.Industry,
                                       SpecificIndustry = er.Employer.SpecificIndustry,
                                       PhoneNumber = er.Employer.PhoneNumber,
                                       Status = er.Employer.Status,
                                       EmployerProfileId = er.UserProfile.Id,
                                       CreatedAt = er.Employer.CreatedAt,
                                       Did = dock.DID
                                   });

            IQueryable<EmployerResponseDTO> employerResponseQuery = enuEmployerResponseQuery.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.PaginationDTO.SearchTerm))
            {
                employerResponseQuery = employerResponseQuery.Where(p =>
                    p.Name.ToLower().Contains(request.PaginationDTO.SearchTerm.ToLower()) ||
                    ((string)p.Email).ToLower().Contains(request.PaginationDTO.SearchTerm.ToLower()) ||
                    ((string)p.Address).ToLower().Contains(request.PaginationDTO.SearchTerm.ToLower()) ||
                    ((string)p.PhoneNumber).ToLower().Contains(request.PaginationDTO.SearchTerm.ToLower()) ||
                    (p.CreatedAt.ToString()).ToLower().Contains(request.PaginationDTO.SearchTerm.ToLower()) ||
                    (p.Status.ToString()).ToLower().Contains(request.PaginationDTO.SearchTerm.ToLower()) 
                    );
            }
            if (request.PaginationDTO.SortOrder?.ToLower() == "desc")
            {
                employerResponseQuery = employerResponseQuery.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                employerResponseQuery = employerResponseQuery.OrderBy(GetSortProperty(request));

            }

            var employerResponseList = await PagedList<EmployerResponseDTO>.CreateAsync(
                            employerResponseQuery,
                            request.PaginationDTO.Page,
            request.PaginationDTO.PageSize);
            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: employerResponseList, message: "Employer list");
        }

        private static Expression<Func<EmployerResponseDTO, object>> GetSortProperty(GetEmployerListQuery request)
        {
            return request.PaginationDTO.SortColumn?.ToLower() switch
            {
                "name" => employer => employer.Name,
                "email" => employer => employer.Email,
                "address" => employer => employer.Address,
                "phoneNumber" => employer => employer.PhoneNumber,
                "status" => employer => employer.Status,
                "CreatedAt" => employer => employer.CreatedAt,
                _ => employer => employer.CreatedAt
            };
        }
    }
}


