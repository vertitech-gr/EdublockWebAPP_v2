using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduUserRequest
{
    public class GetRequestsQuery : IRequest<ApiResponse<object>>
    {
        public PaginationReceivedRequestDTO PaginationReceivedRequestDTO { get; set; }

        public GetRequestsQuery(PaginationReceivedRequestDTO paginationReceivedRequestDTO)
        {
            PaginationReceivedRequestDTO = paginationReceivedRequestDTO;
        }
    }

    public class GetOutgoingRequestsQuery : IRequest<ApiResponse<object>>
    {
        public PaginationGuidDTO PaginationGuidDTO { get; set; }

        public GetOutgoingRequestsQuery(PaginationGuidDTO paginationGuidDTO)
        {
            PaginationGuidDTO = paginationGuidDTO;
        }
    }
}