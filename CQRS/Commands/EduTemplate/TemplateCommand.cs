using Edu_Block_dev.Modal.DTO.EduSchema;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduRole
{
    public class CreateTemplateCommand : IRequest<ApiResponse<object>>
    {
        public TemplateDTO TemplateDTO { get; set; }
        public CreateTemplateCommand(TemplateDTO templateDTO)
        {
            TemplateDTO = templateDTO;
        }
    }
}