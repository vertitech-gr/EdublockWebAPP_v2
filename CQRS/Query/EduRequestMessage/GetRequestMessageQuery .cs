using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduUserRequest
{
    public class GetRequestMessageQuery : IRequest<ApiResponse<object>>
    {
        public TypedPaginationGuidDTO TypedPaginationGuidDTO { get; set; }

        public GetRequestMessageQuery(TypedPaginationGuidDTO typedPaginationGuidDTO)
        {
            TypedPaginationGuidDTO = typedPaginationGuidDTO;
        }
    }
}