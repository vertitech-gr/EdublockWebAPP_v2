using System.Linq.Expressions;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduDepartment;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Helpers;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler
{
    public class GetTemplateListQueryHandler : IRequestHandler<GetTemplateQuery, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly Edu_Block.DAL.IRepository<Template> _templateRepository;

        public GetTemplateListQueryHandler(EduBlockDataContext context, Edu_Block.DAL.IRepository<Template> templateRepository)
        {
            _context = context;
            _templateRepository = templateRepository;
        }

        public async Task<ApiResponse<object>> Handle(GetTemplateQuery request, CancellationToken cancellationToken)
        {

            IEnumerable<Template> enuTemplateQuery = await _templateRepository.GetAllAsync();

            IQueryable<Template> templateResponseQuery = enuTemplateQuery.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.PaginationUniversityDepartmentSchemaDTO.SearchTerm))
            {
                templateResponseQuery = templateResponseQuery.Where(p =>
                    p.Name.ToLower().Contains(request.PaginationUniversityDepartmentSchemaDTO.SearchTerm));
            }
            if (request.PaginationUniversityDepartmentSchemaDTO.SortOrder?.ToLower() == "desc")
            {
                templateResponseQuery = templateResponseQuery.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                templateResponseQuery = templateResponseQuery.OrderBy(GetSortProperty(request));

            }

            if (request.PaginationUniversityDepartmentSchemaDTO.Guid != Guid.Empty)
            {
                templateResponseQuery = templateResponseQuery.Where(d => d.Id == request.PaginationUniversityDepartmentSchemaDTO.Guid);
            }

            if (request.PaginationUniversityDepartmentSchemaDTO.UniversityId != Guid.Empty)
            {
                templateResponseQuery = templateResponseQuery.Where(d => d.UniversityId == request.PaginationUniversityDepartmentSchemaDTO.UniversityId);
            }

            if (request.PaginationUniversityDepartmentSchemaDTO.DepartmentId != Guid.Empty)
            {
                templateResponseQuery = templateResponseQuery.Where(d => d.DepartmentId == request.PaginationUniversityDepartmentSchemaDTO.DepartmentId);
            }

            if (request.PaginationUniversityDepartmentSchemaDTO.SchemaId != Guid.Empty)
            {
                templateResponseQuery = templateResponseQuery.Where(d => d.SchemaId == request.PaginationUniversityDepartmentSchemaDTO.SchemaId);
            }

            var employerResponseList = await PagedList<Template>.CreateAsync(
                            templateResponseQuery,
                            request.PaginationUniversityDepartmentSchemaDTO.Page,
            request.PaginationUniversityDepartmentSchemaDTO.PageSize);

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: employerResponseList, message: "Employer list");
        }

        private static Expression<Func<Template, object>> GetSortProperty(GetTemplateQuery request)
        {
            return request.PaginationUniversityDepartmentSchemaDTO.SortColumn?.ToLower() switch
            {
                "name" => template => template.Name,
                "CreatedAt" => template => template.CreatedAt,
                _ => template => template.CreatedAt
            };
        }
    }
}


