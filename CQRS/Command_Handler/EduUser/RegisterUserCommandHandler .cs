using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using EduBlock.Model.DTO;
using MediatR;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.CQRS.Commands;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<object>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly Util _util;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public RegisterUserCommandHandler(IRepository<UserRole> userRoleRepository, IRepository<User> userRepository, IRepository<Otp> otpRepository, Util util, IMapper mapper, IMediator mediator, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _otpRepository = otpRepository;
            _util = util;
            _mapper = mapper;
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var userDto = request.UserDto;

            if (userDto == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest,message: "Invalid user data");
            }

            var existingUser = await _userRepository.FindAsync(u => u.Email == userDto.Email);

            if (existingUser != null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.Conflict, message: "User with this email already exists");
            }

            User user = _mapper.Map<User>(userDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            User addedUser = _userRepository.Add(user);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string userSpecificInfo = addedUser.Id.ToString();
            string dataToHash = $"{timestamp}-{userSpecificInfo}";
            string key = _util.ComputeSha256Hash(dataToHash);

            UserRole userRole = new UserRole();
            userRole.UserRoleId = Authorization.Role.STUDENT;
            userRole.UserId = addedUser.Id;
            _userRoleRepository.Add(userRole);

            var CreateDockHandlerRresult = await _mediator.Send(new CreateDockHandlerCommand());

            UserProfile userProfile = new UserProfile()
            {
                UserID = user.Id,
                Name = userDto.Name,
                Description = userDto.Name
            };
            var UserProfileCommandResult = await _mediator.Send(new UserProfileCommand(userProfile));
            var UserProfileQueryResult = await _mediator.Send(new GetUserProfileQuery(userProfile));

            Otp otp = new Otp();
            otp.key = key;
            otp.OtpCode = _util.GenerateOtp();
            otp.UserProfileId = UserProfileQueryResult.Id;
            otp.ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value));

            _otpRepository.Add(otp);

            DockIoDID dockDID = new DockIoDID()
            {
                DID = CreateDockHandlerRresult.Did,
                UserProfileId = UserProfileQueryResult.Id,
                Credentials = string.Empty,
                Password = _configuration.GetSection("Dock:Password").Value
            };
            var CreateDockDIDResult = await _mediator.Send(new CertificateDIDCommand(dockDID));


            var subject = "Verify Registration --------- EduBlock";
            var content = $"<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                          $"<br>" +
                          $"<span>OTP : <strong>{otp.OtpCode}</strong></span>";

             _util.SendSimpleMessage(subject, addedUser.Email, content, null, null);

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK,data: new { key }, message: "Verify email with OTP");
        }
    }
}
