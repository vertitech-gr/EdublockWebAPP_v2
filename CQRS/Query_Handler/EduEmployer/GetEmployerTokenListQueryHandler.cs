using System.Linq.Expressions;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduEmployer;
using Edu_Block_dev.Helpers;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler
{
    public class GetEmployerTokenListQueryHandler : IRequestHandler<GetEmployerTokenListQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;

        public GetEmployerTokenListQueryHandler(EduBlockDataContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<object>> Handle(GetEmployerTokenListQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<EmployerToken> enuEmployerResponseQuery = _context.EmployerTokens.Where( e => e.UserId == request.PaginationDTO.guid).ToList();
            IQueryable<EmployerToken> employerResponseQuery = enuEmployerResponseQuery.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.PaginationDTO.SearchTerm))
            {
                employerResponseQuery = employerResponseQuery.Where(p =>
                    p.Name.ToLower().Contains(request.PaginationDTO.SearchTerm));
            }
            if (request.PaginationDTO.SortOrder?.ToLower() == "desc")
            {
                employerResponseQuery = employerResponseQuery.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                employerResponseQuery = employerResponseQuery.OrderBy(GetSortProperty(request));

            }

            var employerResponseList = await PagedList<EmployerToken>.CreateAsync(
                            employerResponseQuery,
                            request.PaginationDTO.Page,
            request.PaginationDTO.PageSize);

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: employerResponseList, message: "Employer list");
        }

        private static Expression<Func<EmployerToken, object>> GetSortProperty(GetEmployerTokenListQuery request)
        {
            return request.PaginationDTO.SortColumn?.ToLower() switch
            {
                "name" => employer => employer.Name,
                "CreatedAt" => employer => employer.CreatedAt,
                _ => employer => employer.CreatedAt
            };
        }
    }
}


