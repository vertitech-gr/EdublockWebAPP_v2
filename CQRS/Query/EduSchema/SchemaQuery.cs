using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduDepartment
{
    public class GetSchemaQuery : IRequest<ApiResponse<object>>
    {
        public PaginationGuidDTO PaginationGuidDTO { get; set; }
        public GetSchemaQuery(PaginationGuidDTO paginationGuidDTO)
        {
            PaginationGuidDTO = paginationGuidDTO;
        }
    }
}