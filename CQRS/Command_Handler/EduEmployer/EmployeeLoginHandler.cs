using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using EduBlock.Model.DTO;
using MediatR;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EduEmployer;
using Edu_Block_dev.DAL.EF;
using System.Net;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block_dev.Modal.DTO;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class EmployeeLoginHandler : IRequestHandler<EmployeeLoginCommand, ApiResponse<object>>
    {
        private readonly IRepository<Employer> _employeeRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<DockIoDID> _dockRepository;
        private readonly Util _util;
        private readonly IJwtUtils _jwtUtils;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public EmployeeLoginHandler(IMediator mediator, IRepository<UserProfile> userProfileRepository, IRepository<Employer> employeeRepository, IRepository<User> userRepository, IRepository<Otp> otpRepository, IRepository<DockIoDID> dockRepository, Util util, IJwtUtils jwtUtils, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _otpRepository = otpRepository;
            _userProfileRepository = userProfileRepository;
            _dockRepository = dockRepository;
            _util = util;
            _jwtUtils = jwtUtils;
            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task<ApiResponse<object>> Handle(EmployeeLoginCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await _employeeRepository.FindAsync(u => u.Email == request._employeeLoginDTO.Email);

            if (existingEmployee == null || !BCrypt.Net.BCrypt.Verify(request._employeeLoginDTO.Password, existingEmployee.Password))
            {
                return new ApiResponse<object>(HttpStatusCode.Unauthorized,data: new { LoginStatus = LoginStatus.Invalid }, message: "Invalid email or password");
            }

            RolesAndPermissionDTO rolesAndPermissionDTO = new RolesAndPermissionDTO();

            UserProfile userProfile = await _userProfileRepository.FindAsync(u => u.UserID == existingEmployee.Id);

            UserRole userRole = await _mediator.Send(new GetUserRoleQuery(existingEmployee.Id));

            DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == userProfile.Id);

            if (userRole != null)
            {
                rolesAndPermissionDTO = await _mediator.Send(new GetRolesAndPermissionQuery(userRole));
            }

            string key = null;

            if (!existingEmployee.Status)
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string userSpecificInfo = existingEmployee.Id.ToString();
                string dataToHash = $"{timestamp}-{userSpecificInfo}";
                key = _util.ComputeSha256Hash(dataToHash);

                Otp otp = new Otp();
                otp.key = key;
                otp.OtpCode = _util.GenerateOtp();
                otp.OtpType = OtpType.REGISTER;
                otp.UserProfileId = userProfile.Id;
                otp.ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value));
                _otpRepository.Add(otp);

                var subject = "Verify Registration --------- EduBlock";
                var content = "<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                              "<br>" +
                              $"<span>OTP: <strong>{otp.OtpCode}</strong></span>";
                _util.SendSimpleMessage(subject, existingEmployee.Email, content, null, null);

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK,data: new { key }, message: "Verify email using OTP");
            }
            var access_token = _jwtUtils.GenerateJwtTokenForEmployee(existingEmployee, string.Empty);
            return new ApiResponse<object>(System.Net.HttpStatusCode.OK,data: new { LoginStatus = LoginStatus.Completed, existingEmployee, rolesAndPermissionDTO, access_token, did = dock.DID }, message: "Employee login successfully");
        }
    }

}
