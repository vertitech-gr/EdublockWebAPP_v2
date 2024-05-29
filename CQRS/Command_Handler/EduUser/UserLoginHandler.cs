using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using EduBlock.Model.DTO;
using MediatR;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EduUser;
using System.Net;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, ApiResponse<object>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<DockIoDID> _dockRepository;
        private readonly Util _util;
        private readonly IJwtUtils _jwtUtils;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IRepository<UserProfile> _userProfileRepository;


        public UserLoginCommandHandler(IMediator mediator, IRepository<UserProfile> userProfileRepository, IRepository<DockIoDID> dockRepository, IRepository<User> userRepository, IRepository<Otp> otpRepository, Util util, IJwtUtils jwtUtils, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _otpRepository = otpRepository;
            _dockRepository = dockRepository;
            _util = util;
            _jwtUtils = jwtUtils;
            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task<ApiResponse<object>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.FindAsync(u => u.Email == request.Email);
            if (existingUser == null || !BCrypt.Net.BCrypt.Verify(request.Password, existingUser.Password))
            {
                return new ApiResponse<object>(HttpStatusCode.Unauthorized,error: "false", message: "Invalid email or password");
            }
            if (!existingUser.loginStatus)
            {
                return new ApiResponse<object>(HttpStatusCode.BadRequest, error: "false", message: "Login Disabled, Please Contact Support");
            }
            RolesAndPermissionDTO rolesAndPermissionDTO = new RolesAndPermissionDTO();
            UserProfile userProfile = await _userProfileRepository.FindAsync(u => u.UserID == existingUser.Id);
            UserRole userRole = await _mediator.Send(new GetUserRoleQuery(existingUser.Id));
            DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == userProfile.Id);
            if (userRole != null)
            {
                rolesAndPermissionDTO = await _mediator.Send(new GetRolesAndPermissionQuery(userRole));
            }
            string key = null;
            if (!existingUser.Status)
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string userSpecificInfo = existingUser.Id.ToString();
                string dataToHash = $"{timestamp}-{userSpecificInfo}";
                key = _util.ComputeSha256Hash(dataToHash);
                Otp otp = new Otp();
                otp.key = key;
                otp.OtpCode = _util.GenerateOtp();
                otp.OtpType = OtpType.REGISTER;
                otp.UserProfileId = existingUser.Id;
                otp.ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value));
                _otpRepository.Add(otp);
                var subject = "Verify Registration --------- EduBlock";
                var content = "<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                              "<br>" +
                              $"<span>OTP: <strong>{otp.OtpCode}</strong></span>";
                 _util.SendSimpleMessage(subject, existingUser.Email, content);
                return new ApiResponse<object>(HttpStatusCode.OK, data: new { key }, message: "Verify email using OTP");
            }
            //_jwtUtils.GenerateJwtToken(user)
            var access_token = _jwtUtils.GenerateJwtToken(existingUser);
            return new ApiResponse<object>(HttpStatusCode.OK, data: new { LoginStatus = LoginStatus.Completed, existingUser, userProfile, rolesAndPermissionDTO, access_token, did = dock.DID }, message: "User login successfully");
        }
    }

}
