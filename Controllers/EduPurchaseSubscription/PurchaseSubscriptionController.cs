using Edu_Block_dev.CQRS.Commands.EduPurchaseSubscription;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.Authorization;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Edu_Block_dev.CQRS.Query.EduPurchaseSubscription;

namespace Edu_Block_dev.Controllers.EduPurchaseSubscription
{
    [Route("api/purchase-subscription")]
    [ApiController]
    public class PurchaseSubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchaseSubscriptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create")]
        [Authorize("PURCHASESUBSCRIPTION","CREATE")]
        public async Task<IActionResult> PurchaseSubscription([FromBody] PurchaseSubscriptionDTO purchaseSubscriptionDto)
        {
            try
            {
                CommanUser? user = HttpContext.Items["User"] as CommanUser;
                var command = new PurchaseSubscriptionCommand(purchaseSubscriptionDto, user);
                var result = await _mediator.Send(command);

                return Ok(new ApiResponse<object>(System.Net.HttpStatusCode.OK ,data: result, message: "Subscription purchased successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>( System.Net.HttpStatusCode.InternalServerError ,message: $"Error purchasing subscription: {ex.Message}"));
            }
        }

        [HttpGet("get")]
        [Authorize("PURCHASESUBSCRIPTION", "LIST")]
        public async Task<IActionResult> GetAllPurchaseSubscriptions([FromQuery] PaginationGuidDTO paginationGuidDTO)
        {
                var result = await _mediator.Send(new PaginationPurchaseSubscriptionQuery(paginationGuidDTO));
                return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("coin")]
        public async Task<IActionResult> PurchaseCoins([FromBody] PurchaseCoinsDTO purchaseCoinsDTO)
        {
            try
            {
                CommanUser? user = HttpContext.Items["User"] as CommanUser;
                var command = new PurchaseCoinsCommand(purchaseCoinsDTO, user);
                var result = await _mediator.Send(command);

                return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: result, message: "Coin Deduct Successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: $"An error occurred: {ex.Message}"));

            }
        }
    }
}
