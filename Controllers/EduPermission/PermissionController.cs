using Microsoft.AspNetCore.Mvc;
using MediatR;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.CQRS.Commands.EduPermission;
using Edu_Block_dev.CQRS.Query.EduPurchaseSubscription;
using Edu_Block_dev.Authorization;

namespace Edu_Block_dev.Controllers.EduPermission
{
    [ApiController]
    [Route("api/permission")]
    public class PermissionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddPermission([FromBody] PermissionRequestDTO permissionRequestDTO)
        {
            var result = await _mediator.Send(new AddPermissionCommand(permissionRequestDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get")]
        [Authorize("PERMISSION", "GET")]
        public async Task<IActionResult> GetPermission(Guid guid)
        {
            CommanUser user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new GetPermissionQuery(guid, user));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}