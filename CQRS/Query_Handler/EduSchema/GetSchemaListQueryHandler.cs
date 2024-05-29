using System.Linq.Expressions;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduDepartment;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Helpers;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler
{
    public class GetSchemaListQueryHandler : IRequestHandler<GetSchemaQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly Edu_Block.DAL.IRepository<Schema> _schemaRepository;

        public GetSchemaListQueryHandler(EduBlockDataContext context, Edu_Block.DAL.IRepository<Schema> schemaRepository)
        {
            _context = context;
            _schemaRepository = schemaRepository;
        }

        public async Task<ApiResponse<object>> Handle(GetSchemaQuery request, CancellationToken cancellationToken)
        {


            IEnumerable<Schema> enuSchemaQuery = await _schemaRepository.GetAllAsync();

            IQueryable<Schema> schemaResponseQuery = enuSchemaQuery.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.PaginationGuidDTO.SearchTerm))
            {
                schemaResponseQuery = schemaResponseQuery.Where(p =>
                    p.Name.ToLower().Contains(request.PaginationGuidDTO.SearchTerm));
            }
            if (request.PaginationGuidDTO.SortOrder?.ToLower() == "desc")
            {
                schemaResponseQuery = schemaResponseQuery.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                schemaResponseQuery = schemaResponseQuery.OrderBy(GetSortProperty(request));

            }

            if (request.PaginationGuidDTO.guid != Guid.Empty)
            {
                schemaResponseQuery = schemaResponseQuery.Where(d => d.UniversityId == request.PaginationGuidDTO.guid);
            }


            var employerResponseList = await PagedList<Schema>.CreateAsync(
                            schemaResponseQuery,
                            request.PaginationGuidDTO.Page,
            request.PaginationGuidDTO.PageSize);



            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: employerResponseList, message: "Employer list");
        }

        private static Expression<Func<Schema, object>> GetSortProperty(GetSchemaQuery request)
        {
            return request.PaginationGuidDTO.SortColumn?.ToLower() switch
            {
                "name" => template => template.Name,
                "CreatedAt" => template => template.CreatedAt,
                _ => template => template.CreatedAt
            };
        }
    }
}


