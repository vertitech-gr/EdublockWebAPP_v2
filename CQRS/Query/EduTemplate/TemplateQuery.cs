using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduDepartment
{
    public class GetTemplateQuery : IRequest<ApiResponse<object>>
    {
        public PaginationUniversityDepartmentSchemaDTO PaginationUniversityDepartmentSchemaDTO { get; set; }
        public GetTemplateQuery(PaginationUniversityDepartmentSchemaDTO paginationUniversityDepartmentSchemaDTO)
        {
            PaginationUniversityDepartmentSchemaDTO = paginationUniversityDepartmentSchemaDTO;
        }
    }
}