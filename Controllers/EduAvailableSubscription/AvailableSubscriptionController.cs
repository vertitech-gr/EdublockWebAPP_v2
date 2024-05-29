using Edu_Block_dev.CQRS.Commands.EduAvailableSubscription;
using Edu_Block_dev.CQRS.Query.EduAvailableSubscription;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;
using Edu_Block_dev.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Block_dev.Controllers.EduAvailableSubscription
{
    [Route("api/available-subscription")]
    [ApiController]
    public class AvailableSubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AvailableSubscriptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        [Authorize("AVAILABLESUBSCRIPTION", "Create")]
        public async Task<IActionResult> CreateAvailableSubscription([FromBody] AvailableSubscriptionDto availableSubscriptionDto)
        {
            try
            {
                CommanUser? user = HttpContext.Items["User"] as CommanUser;
                var result = await _mediator.Send(new AvailableSubscriptionCommand(availableSubscriptionDto, user));
                return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.Created, data: result, message: "Create Successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError ,message: $"Error creating AvailableSubscription: {ex.Message}"));
            }
        }

        [HttpPatch("update")]
        [Authorize("AVAILABLESUBSCRIPTION", "Update")]
        public async Task<IActionResult> UpdateAvailableSubscription([FromBody] EditSubscriptionDto editSubscriptionDto)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new EditSubscriptionCommand(editSubscriptionDto, user));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("delete")]
        [Authorize("AVAILABLESUBSCRIPTION", "Delete")]
        public async Task<IActionResult> Delete(Guid guid)
        {
                CommanUser? user = HttpContext.Items["User"] as CommanUser;
                var result = await _mediator.Send(new DeleteAvailableSubscriptionCommand(guid, user));
                return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get/{Id}")]
        public async Task<IActionResult> GetAllAvailableSubscriptions(Guid Id)
        {
            try
            {
                var result = await _mediator.Send(new GetAvailableSubscriptionsByIdQuery(Id));
                if(result == null)
                {
                    return StatusCode(400, new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "Not Found"));
                }
                return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: result, message: "retrieve Successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: $"Error retrieving Available Subscriptions: {ex.Message}"));

            }
        }

        [HttpGet ("get")]
        public async Task<IActionResult> GetAllAvailableSubscriptions([FromQuery] PaginationGuidDTO paginationGuidDTO)
        {
                var result = await _mediator.Send(new GetAllAvailableSubscriptionsQuery(paginationGuidDTO));
                return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
