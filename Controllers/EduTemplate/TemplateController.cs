using Edu_Block_dev.CQRS.Commands.EduRole;
using Edu_Block_dev.CQRS.Query.EduDepartment;
using Edu_Block_dev.Modal.DTO.EduSchema;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Block_dev.Controllers.EmployeeRequest
{
    [ApiController]
    [Route("api/template")]
    public class TemplateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TemplateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddTemplate([FromBody] TemplateDTO templateDTO)
        {
            var result = await _mediator.Send(new CreateTemplateCommand(templateDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetTemplate([FromQuery] PaginationUniversityDepartmentSchemaDTO PaginationUniversityDepartmentSchemaDTO)
        {
            var result = await _mediator.Send(new GetTemplateQuery(PaginationUniversityDepartmentSchemaDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}