using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Command_Handler.EduUser;
using Edu_Block_dev.CQRS.Commands.EduEmployer;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.CQRS.Query.EduEmployer;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Role = Edu_Block_dev.Authorization.Role;

namespace Edu_Block_dev.Controllers.EduEmployer
{
    [ApiController]
    [Route("api/employer")]
    public class EmployerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Employer> _employeeRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly Util _util;
        private readonly IJwtUtils _jwtUtils;

        public EmployerController(IRepository<UserProfile> userProfileRepository, IRepository<Employer> employeeRepository, IRepository<Otp> otpRepository, Util util, IMediator mediator, IJwtUtils jwtUtils)
        {
            _userProfileRepository = userProfileRepository;
            _employeeRepository = employeeRepository;
            _otpRepository = otpRepository;
            _util = util;
            _mediator = mediator;
            _jwtUtils = jwtUtils;
        }

        [HttpPost]
        [Route("register-employee")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterEmployee([FromBody] EmployerDTO employerDTO)
        {
            var result = await _mediator.Send(new RegisterEmployeeCommand(employerDTO));
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost]
        [Route("create-employee-token")]
        [Authorize("EMPLOYER_TOKEN", "CREATE")]
        public async Task<IActionResult> CreateEmployeeToken(string name)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new CreateEmployeeTokenCommand(name, user));
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPatch]
        [Route("delete-employee-token")]
        [Authorize("EMPLOYER_TOKEN", "DELETE")]
        public async Task<IActionResult> CreateEmployeeToken(Guid empTokenId)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new DeleteEmployeeTokenCommand(empTokenId, user));
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        [Route("verify-employee-registration")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmployeeEmail([FromBody] VerifyEmailDTO verifyEmailDto)
        {
            var commandHandler = new VerifyEmployeeEmailCommandHandler(_userProfileRepository, _otpRepository, _employeeRepository, _jwtUtils);
            var command = new VerifyEmployeeEmailCommand { VerifyEmailDto = verifyEmailDto };
            var result = await commandHandler.Handle(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        [Route("edit-employee")]
        [Authorize("EMPLOYER", "Edit")]
        public async Task<IActionResult> EditEmployeeDetails( EditEmployerDTO editEmployerDTO)
        {
            var result = await _mediator.Send(new EditEmployerCommand(editEmployerDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("resend-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp(string Email)
        {
            var result = await _mediator.Send(new ResendOtpCommand(Email, Role.EMPLOYER));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("employee-login")]
        [AllowAnonymous]
        public async Task<IActionResult> EmployeeLogin([FromBody] EmployeeLoginDTO employeeLoginDTO )
        {
            var result = await _mediator.Send(new EmployeeLoginCommand(employeeLoginDTO));
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        [Route("emp-change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> EmployeeChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            var result = await _mediator.Send(new EmployeeChangePasswordCommand(changePasswordDto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("details")]
        [Authorize("EMPLOYER", "GET", true)]
        public async Task<IActionResult> GetEmployerDetails()
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            if (user != null)
            {
                return new OkObjectResult(new ApiResponse<object>( System.Net.HttpStatusCode.OK ,data: user, message: "User details retrieved successfully"));
            }
            return NotFound("User not found");
        }

        [HttpGet("employer-list")]
        [Authorize("EMPLOYER", "LIST")]
        public async Task<IActionResult> GetEmployers([FromQuery] PaginationDTO paginationDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new GetEmployerListQuery(user, paginationDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("employer-token-list")]
        [Authorize("EMPLOYER_TOKEN", "LIST")]
        public async Task<IActionResult> GetEmployersTokenList([FromQuery] PaginationGuidDTO paginationDTO)
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var result = await _mediator.Send(new GetEmployerTokenListQuery(user, paginationDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("forget-password/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetEmployeePassword(string email)
        {
            var result = await _mediator.Send(new ForgetEmployeePasswordCommand(email));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployer(Guid id)
        {
            var employerDTO = await _mediator.Send(new GetEmployeeByUniqueIdQuery(id));
            if (employerDTO == null)
                return NotFound();
            return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: employerDTO, message: "Employee retrieved successfully"));
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetEmployerByProfileId(Guid id)
        {
            var employerDTO = await _mediator.Send(new GetEmployeeByProfileIdQuery(id));
            if (employerDTO == null)
                return NotFound();
            return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: employerDTO, message: "Employee retrieved successfully"));
        }

    }

}
