using Edu_Block.DAL.EF;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AutoMapper;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.DAL.EF;
using EduBlock.Model.DTO;
using Edu_Block_dev.CQRS.Commands.EduShareCredential;
using Edu_Block_dev.Controllers.EduWebhook;

namespace Edu_Block_dev.Controllers.EduUniversity
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentController> _logger;


        public PaymentController(EduBlockDataContext context, IMediator mediator, IMapper mapper, ILogger<PaymentController> logger)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        //[Authorize(Authorization.Role.EMPLOYER)]
        [Route("payment-url-generator")]
        [Authorize("PAYMENT","URL_GENERATOR")]
        public async Task<IActionResult> SharePaymentUrl([FromBody] PaymentUrlDTO paymentUrlDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new PaymentUrlCommand(user ,paymentUrlDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}