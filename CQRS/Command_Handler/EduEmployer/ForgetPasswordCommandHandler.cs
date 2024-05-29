using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduEmployer;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class ForgetEmployeePasswordCommandHandler: IRequestHandler<ForgetEmployeePasswordCommand, ApiResponse<object>>
    {
        private readonly IRepository<DAL.EF.Employer> _employerRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly Util _util;
        private readonly IConfiguration _configuration;

        public ForgetEmployeePasswordCommandHandler(IRepository<UserProfile> userProfileRepository, IRepository<DAL.EF.Employer> employerRepository, IRepository<Otp> otpRepository, Util util, IConfiguration configuration)
        {
            _employerRepository = employerRepository;
            _otpRepository = otpRepository;
            _userProfileRepository = userProfileRepository;
            _util = util;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(ForgetEmployeePasswordCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await _employerRepository.FindAsync(u => u.Email == command.Email);
            if (existingUser == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, data: null, message: "unable to send Email");
            }
            var existingUserProfile = await _userProfileRepository.FindAsync(u => u.UserID == existingUser.Id);
            if (existingUser == null || existingUserProfile == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, data: null, message: "unable to send Email");
            }
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string userSpecificInfo = existingUser.Id.ToString();
            string dataToHash = $"{timestamp}-{userSpecificInfo}";
            var key = _util.ComputeSha256Hash(dataToHash);
            Otp otp = new Otp
            {
                key = key,
                OtpCode = _util.GenerateOtp(),
                OtpType = OtpType.FORGET_PASSWORD,
                UserProfileId = existingUserProfile.Id,
                ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value))
             };
            _otpRepository.Add(otp);
            var subject = "Reset Password --------- EduBlock";
            var content = "<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                          "<br>" +
                          "<span>OTP : <strong>" + otp.OtpCode + "</strong></span>";
            _util.SendSimpleMessage(subject, command.Email, content, null, null );
            var responseData = new { key };

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: responseData, message: $"OTP Sent To {command.Email}");
        }
    }
}
