using Microsoft.AspNetCore.Mvc;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduRole;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.CQRS.Query.EduPurchaseSubscription;
using Edu_Block_dev.Authorization;

namespace Edu_Block_dev.Controllers.EduRole
{
    [ApiController]
    [Route("api/role")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddRoleCreate([FromBody] RoleRequestDTO roleRequestDTO)
        {
            var result = await _mediator.Send(new AddRoleCommand(roleRequestDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequestDTO role)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(role));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteRole(Guid guid)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(guid));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoles(Guid guid)
        {
            CommanUser user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new GetRoleQuery(guid,user));
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}