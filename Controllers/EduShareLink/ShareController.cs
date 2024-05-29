using Microsoft.AspNetCore.Mvc;
using MediatR;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.CQRS.Commands.EduShareLink;
using Edu_Block_dev.CQRS.Query.EduShareLink;
using Edu_Block_dev.Authorization;

namespace Edu_Block_dev.Controllers.EduShareLink
{
    [ApiController]
    [Route("api/share")]
    public class ShareController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShareController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        //[Authorize(Authorization.Role.STUDENT)]
        [Authorize("SHARERESOURCE","CREATE")]
        public async Task<IActionResult> ShareResource([FromBody] ShareDTO dto)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new ShareCommand(dto, user));
            return result;
        }

        [HttpGet("share-by-uniqueId/{uniqueId}")]
        public async Task<IActionResult> GetShareByUniqueId(Guid uniqueId)
        {
            var query = new ShareByUniqueIdQuery(uniqueId);
            var shareDTO = await _mediator.Send(query);
            if (shareDTO != null)
            {
                var response = new ApiResponse<object>( System.Net.HttpStatusCode.OK, shareDTO, "Share retrieved successfully");
                return new OkObjectResult(response);
            }
            else
            {
                var response = new ApiResponse<object>( System.Net.HttpStatusCode.NotFound, data: null, message: "Share not found");
                return NotFound(response);
            }
        }

        [HttpPost("share-verification")]
        [Authorize("SHARERESOURCE","VERIFICATION")]
        public async Task<IActionResult> ShareVerification([FromBody] ShareVerificationDTO verificationDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new ShareVerificationCommand(verificationDTO, user));
            return result != null ? Ok(result) : BadRequest(result);
        }
    }
}