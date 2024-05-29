using Microsoft.AspNetCore.Mvc;
using MediatR;
using Edu_Block_dev.Authorization;
using EduBlock.Model.DTO;
using Edu_Block_dev.CQRS.Commands.EduUser;
using System.Net;
using Edu_Block_dev.CQRS.Query.EduTransaction;
using Edu_Block_dev.Modal.DTO;

namespace Edu_Block_dev.Controllers.EduUniversity
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("register-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserDTO UserDto)
        {
            var result = await _mediator.Send(new RegisterAdminCommand(UserDto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("verify-admin-registration")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyAdminEmail([FromBody] VerifyEmailDTO verifyEmailDto)
        {
            var result = await _mediator.Send(new VerifyAdminEmailCommand(verifyEmailDto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("admin-login")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDTO loginDTO)
        {
            var result = await _mediator.Send(new AdminLoginCommand(loginDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("change-admin-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeAdminPassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            var result = await _mediator.Send(new ChangeAdminPasswordCommand(changePasswordDto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("resend-admin-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendAdminOtp(string Email)
        {
            var result = await _mediator.Send(new ResendAdminOtpCommand(Email));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("admin-details")]
        [Authorize("ADMIN", "GetAdminDetails")]
        public async Task<IActionResult> GetAdminDetails()
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            if (user != null)
            {
                return new OkObjectResult(new ApiResponse<object>(HttpStatusCode.OK, data: user, message: "University details retrieved successfully"));
            }
            return NotFound("University not found");
        }

        [HttpGet("all-transaction-list")]
         [Authorize("ADMIN", "TRANSACTIONS")]
        public async Task<IActionResult> GetAllTransactions([FromQuery] PaginationGuidDTO transactionPaginationDTO)
        {
            CommanUser user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new AllTransactionQuery(user, transactionPaginationDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}