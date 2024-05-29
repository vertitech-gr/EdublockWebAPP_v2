using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduUserRequest;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block_dev.DAL.EF;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Block_dev.Controllers.EduWebhook
{
    [ApiController]
    [Route("api/transaction")]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IMediator _mediator;
        private readonly EduBlockDataContext _context;

        public WebhookController(EduBlockDataContext context, ILogger<WebhookController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _context = context;
        }

        [HttpPost]
        [Route("payment")]
        public async Task<IActionResult> HandlePaymentWebhookAsync([FromBody] WebhookPayload WebhookPayloadRequest)
        {
            try
            {
                PaymentDetail paymentDetail = new PaymentDetail()
                {
                    Action = WebhookPayloadRequest.action,
                    Application = WebhookPayloadRequest.application,
                    Client = WebhookPayloadRequest.client,
                    Contract = WebhookPayloadRequest.contract,
                    InstanceKey = WebhookPayloadRequest.instance_key,
                    Key = WebhookPayloadRequest.key,
                    Payment = WebhookPayloadRequest.payment,
                    Transaction = WebhookPayloadRequest.transaction
                };
                await _mediator.Send(new PaymentDetailsCommand(paymentDetail));
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error processing payment webhook {ex.Message}");
                return StatusCode(500, "An error occurred while processing the webhook");
            }
        }

        [HttpGet]
        [Route("get-transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] PaginationGuidDTO PaginationGuidDTO)
        {
            try
            {
                var result = await _mediator.Send(new GetPaymentTransactionQuery(PaginationGuidDTO));
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error processing payment webhook {ex.Message}");
                return StatusCode(500, "An error occurred while processing the webhook");
            }
        }

    }

    public class WebhookPayload
    {
        public string key { get; set; }
        public string application { get; set; }
        public string instance_key { get; set; }
        public string client { get; set; }
        public string action { get; set; }
        public string payment { get; set; }
        public string transaction { get; set; }
        public string contract { get; set; }
        public DateTime created { get; set; }
    }
}
