using Edu_Block_dev.CQRS.Query.EduTransaction;
using Edu_Block_dev.DAL.EF;
using EduBlock.Model.DTO;
using MediatR;
using Edu_Block_dev.Authorization;
using Microsoft.AspNetCore.Mvc;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.CQRS.Commands.EduPurchaseSubscription;

namespace Edu_Block_dev.Controllers.EduTransection
{
    [Route("api")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BalanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-balance")]
        //[Authorize(Authorization.Role.EMPLOYER)]
        [Authorize("BALANCE","GET")]
        public async Task<ActionResult<decimal>> GetBalance()
        {
            try
            {
                CommanUser? user = HttpContext.Items["User"] as CommanUser;
                var balance = await _mediator.Send(new GetBalanceQuery(user));
                return Ok(new ApiResponse<object>( System.Net.HttpStatusCode.OK , data: balance, message: "Balance Retrieve  Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: $"An error occurred: {ex.Message}"));
            }
        }

        [HttpPost("purchase-transaction")]
       // [Authorize(Authorization.Role.EMPLOYER)]
        [Authorize("BALANCE","PURCHASE_COINS")]
        public async Task<ActionResult<Transactions>> PurchaseTransaction([FromBody] PurchaseCoinsDTO purchaseCoinsDTO)
        {
            try
            {
                CommanUser? user = HttpContext.Items["User"] as CommanUser;
                var transaction = await _mediator.Send(new PurchaseCoinsCommand(purchaseCoinsDTO, user));
                return Ok(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: transaction, message: "Transection Create Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: $"An error occurred: {ex.Message}"));
            }
        }

        [HttpGet("transaction-list")]
      //  [Authorize(Authorization.Role.EMPLOYER)]
        public async Task<ActionResult<List<TransactionResponseDTO>>> GetTransactions()
        {
            CommanUser user = HttpContext.Items["User"] as CommanUser;
            List<TransactionResponseDTO> transactionList = await _mediator.Send(new TransactionQuery(user));
            return Ok(new ApiResponse<object>( System.Net.HttpStatusCode.OK ,data: transactionList, message: "Transection List Retrieve Successfully"));
        }

    }
}