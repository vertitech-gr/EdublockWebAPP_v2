using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class ForgetPasswordCommandHandler
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly Util _util;
        private readonly IConfiguration _configuration;

        public ForgetPasswordCommandHandler(IRepository<UserProfile> userProfileRepository,IRepository<User> userRepository, IRepository<Otp> otpRepository, Util util, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _userProfileRepository = userProfileRepository;
            _util = util;
            _configuration = configuration;
        }

        public async Task<IActionResult> Handle(ForgetPasswordCommand command)
        {
            var existingUser = await _userRepository.FindAsync(u => u.Email == command.Email);
            if (existingUser == null)
            {
                return new BadRequestObjectResult("Invalid email");
            }
            var existingUserProfile = await _userProfileRepository.FindAsync(u => u.UserID == existingUser.Id);
            if (existingUser == null || existingUserProfile == null)
            {
                return new BadRequestObjectResult("Invalid email");

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
            _util.SendSimpleMessage(subject, command.Email, content, null, null);
            var responseData = new { key };

            return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: responseData, message: $"OTP Sent To {command.Email}"));
        }
    }
}
