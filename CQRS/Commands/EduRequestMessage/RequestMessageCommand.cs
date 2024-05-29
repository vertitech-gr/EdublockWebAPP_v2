using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduRole
{
    public class RequestMessageCommand : IRequest<ApiResponse<object>>
    {
        public RequestMessageDTO RequestMessageDTO { get; set; }
        public RequestMessageCommand(RequestMessageDTO requestMessageDTO)
        {
            RequestMessageDTO = requestMessageDTO;
        }
    }

    
}