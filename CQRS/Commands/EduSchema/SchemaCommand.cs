using Edu_Block_dev.Modal.DTO.EduSchema;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduRole
{
    public class CreateSchemaCommand : IRequest<ApiResponse<object>>
    {
        public SchemaDTO SchemaDTO { get; set; }
        public CreateSchemaCommand(SchemaDTO schemaDTO)
        {
            SchemaDTO = schemaDTO;
        }
    }
}