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
    public class AdminLoginHandler : IRequestHandler<AdminLoginCommand, ApiResponse<object>>
    {
        private readonly IRepository<Admin> _adminRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<DockIoDID> _dockRepository;
        private readonly Util _util;
        private readonly IMediator _mediator;
        private readonly IJwtUtils _jwtUtils;
        private readonly IConfiguration _configuration;

        public AdminLoginHandler(IMediator mediator, IRepository<UserProfile> userProfileRepository, IRepository<Admin> adminRepository, IRepository<Otp> otpRepository, IRepository<DockIoDID> dockRepository, Util util, IJwtUtils jwtUtils, IConfiguration configuration)
        {
            _adminRepository = adminRepository;
            _otpRepository = otpRepository;
            _userProfileRepository = userProfileRepository;
            _dockRepository = dockRepository;
            _util = util;
            _jwtUtils = jwtUtils;
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
        {
            var existingAdmin = await _adminRepository.FindAsync(u => u.Email == request._loginDTO.Email);

            if (existingAdmin == null || !BCrypt.Net.BCrypt.Verify(request._loginDTO.Password, existingAdmin.Password))
            {
                return new ApiResponse<object>(HttpStatusCode.Unauthorized, data: new { LoginStatus = LoginStatus.Invalid }, message: "Invalid email or password");
            }

            RolesAndPermissionDTO rolesAndPermissionDTO = new RolesAndPermissionDTO();

            UserProfile userProfile = await _userProfileRepository.FindAsync(u => u.UserID == existingAdmin.Id);

            UserRole userRole = await _mediator.Send(new GetUserRoleQuery(userProfile.Id));

            DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == userProfile.Id);

            if (userRole != null)
            {
                rolesAndPermissionDTO = await _mediator.Send(new GetRolesAndPermissionQuery(userRole));
            }

            string key = null;

            if (!existingAdmin.Status)
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string userSpecificInfo = existingAdmin.Id.ToString();
                string dataToHash = $"{timestamp}-{userSpecificInfo}";
                key = _util.ComputeSha256Hash(dataToHash);

                UserProfile employeeProfile = await _userProfileRepository.FindAsync(u => u.UserID == existingAdmin.Id);

                Otp otp = new Otp();
                otp.key = key;
                otp.OtpCode = _util.GenerateOtp();
                otp.OtpType = OtpType.REGISTER;
                otp.UserProfileId = employeeProfile.Id;
                otp.ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value));
                _otpRepository.Add(otp);

                var subject = "Verify Registration --------- EduBlock";
                var content = "<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                              "<br>" +
                              $"<span>OTP: <strong>{otp.OtpCode}</strong></span>";

                _util.SendSimpleMessage(subject, existingAdmin.Email, content, null, null);

                return new ApiResponse<object>(HttpStatusCode.OK, data: new { key }, message: "Verify email using OTP");
            }
            var access_token = _jwtUtils.GenerateJwtTokenForAdmin(existingAdmin);
            return new ApiResponse<object>(HttpStatusCode.OK, data: new { LoginStatus = LoginStatus.Completed, existingAdmin, rolesAndPermissionDTO, access_token, did = dock.DID }, message: "Admin login successfully");
        }
    }

}
