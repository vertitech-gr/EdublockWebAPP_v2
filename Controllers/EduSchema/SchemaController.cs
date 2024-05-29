using Edu_Block_dev.CQRS.Commands.EduRole;
using Edu_Block_dev.CQRS.Query.EduDepartment;
using Edu_Block_dev.Modal.DTO.EduSchema;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Block_dev.Controllers.EmployeeRequest
{
    [ApiController]
    [Route("api/schema")]
    public class SchemaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchemaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddSchema([FromBody] SchemaDTO schemaDTO)
        {
            var result = await _mediator.Send(new CreateSchemaCommand(schemaDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetSchema([FromQuery] PaginationGuidDTO PaginationGuidDTO)
        {
            var result = await _mediator.Send(new GetSchemaQuery(PaginationGuidDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}