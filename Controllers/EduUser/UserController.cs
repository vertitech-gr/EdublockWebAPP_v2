using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using Microsoft.AspNetCore.Mvc;
using EduBlock.Model.DTO;
using AutoMapper;
using MediatR;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.CQRS.Command_Handler.EduUser;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block_dev.DAL.EF;
using System.Net;
using Role = Edu_Block_dev.Authorization.Role;
using Edu_Block_dev.Modal.DTO;

namespace Edu_Block_dev.Controllers.EduUser
{
    [Route("api/auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<DockIoDID> _dockRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IMapper _mapper;
        private readonly Util _util;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public UserController(IRepository<UserProfile> userProfileRepository, IRepository<UserRole> userRoleRepository, IRepository<DockIoDID> dockRepository, IRepository<User> userRepository, IRepository<Otp> otpRepository, IMapper mapper, IConfiguration configuration, Util util, IMediator mediator, IJwtUtils jwtUtils)
        {
            _dockRepository = dockRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
            _otpRepository = otpRepository;
            _util = util;
            _mediator = mediator;
            _jwtUtils = jwtUtils;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] UserDTO userDto)
        {
            var commandHandler = new RegisterUserCommandHandler(_userRoleRepository, _userRepository, _otpRepository, _util, _mapper, _mediator, _configuration);
            var command = new RegisterUserCommand { UserDto = userDto };
            var result = await commandHandler.Handle(command, CancellationToken.None);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        [Route("verify-registration")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO verifyEmailDto)
        {
            var commandHandler = new VerifyEmailCommandHandler(_userProfileRepository, _otpRepository, _userRepository, _util, _jwtUtils);
            var command = new VerifyEmailCommand { VerifyEmailDto = verifyEmailDto };
            var result = await commandHandler.Handle(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser([FromBody] LoginDTO userLoginDto)
        {
            if (userLoginDto == null)
            {
                return BadRequest("Invalid login data");
            }
            var userLoginCommandHandler = new UserLoginCommandHandler(_mediator, _userProfileRepository, _dockRepository, _userRepository, _otpRepository, _util, _jwtUtils, _configuration);
            var result = await userLoginCommandHandler.Handle(new UserLoginCommand
            {
                Email = userLoginDto.Email,
                Password = userLoginDto.Password
            }, CancellationToken.None);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }


        [HttpPost]
        [Route("forget-password/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateToken(string email)
        {
            var forgetPasswordCommand = new ForgetPasswordCommand(email);
            var forgetPasswordCommandHandler = new ForgetPasswordCommandHandler(_userProfileRepository, _userRepository, _otpRepository, _util, _configuration);
            var result = await forgetPasswordCommandHandler.Handle(forgetPasswordCommand);
            return result;
        }

        [HttpPost]
        [Route("change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            var changePasswordCommand = new ChangePasswordCommand(changePasswordDto);
            var changePasswordCommandHandler = new ChangePasswordCommandHandler(_userProfileRepository, _userRepository, _otpRepository);
            var result = await changePasswordCommandHandler.Handle(changePasswordCommand);
            return result;
        }

        [HttpPost("resend-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp(string Email)
        {
            var result = await _mediator.Send(new ResendOtpCommand(Email, Role.STUDENT));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("details")]
        [Authorize("STUDENT","GET")]
        public async Task<IActionResult> GetUserDetails()
        {
            CommanUser? user = HttpContext.Items["User"] as CommanUser;
            var existingUser = new User
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt,
                Email = user.Email,
                loginStatus = user.LoginStatus,
                Name = user.Name,
                Status = user.Status,
                UpdatedAt = user.UpdatedAt
            };
            if (user != null)
            {
                return new OkObjectResult(new ApiResponse<object>(HttpStatusCode.OK, data: new { LoginStatus = LoginStatus.Completed, existingUser, user.UserProfile, user.RolesAndPermissionDTO, user.UserRole, user.DID, user.universityResponseDTO }, message: "User details retrieved successfully"));
            }
            return NotFound("User not found");
        }

        [HttpGet("get-users")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationUniversityUserDTO PaginationUniversityUserDTO)
        {
            var result = await _mediator.Send(new GetUsersQuery(PaginationUniversityUserDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-university-users")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUniversityUsers([FromQuery] PaginationUniversityUserDTO PaginationUniversityUserDTO)
        {
            var result = await _mediator.Send(new GetUniversityUsersQuery(PaginationUniversityUserDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-users-list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsersList([FromQuery] PaginationGuidDTO PaginationGuidDTO)
        {
            var result = await _mediator.Send(new GetNewUsersQuery(PaginationGuidDTO));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-user-certifcates")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsersCertificates( Guid userProfileId )
        {
            var result = await _mediator.Send(new GetUserCertificatesQuery(userProfileId));
            return Ok(result);
        }

        [HttpPost("login-status/{type}/{email}")]
        [Authorize("STUDENT", "LOGIN_STATUS")]
        public async Task<IActionResult> UpdateUserLoginStatus(bool type, string email)
        {
            var result = await _mediator.Send(new UpdateUserLoginStatus(type, email));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}