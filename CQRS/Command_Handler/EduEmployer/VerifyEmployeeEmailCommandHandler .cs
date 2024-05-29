using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduEmployer;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class VerifyEmployeeEmailCommandHandler
    {
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<Employer> _employerRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IJwtUtils _jwtUtils;

        public VerifyEmployeeEmailCommandHandler(IRepository<UserProfile> userProfileRepository, IRepository<Otp> otpRepository, IRepository<Employer> employerRepository, IJwtUtils jwtUtils)
        {
            _otpRepository = otpRepository;
            _employerRepository = employerRepository;
            _userProfileRepository = userProfileRepository;
            _jwtUtils = jwtUtils;
        }

        public async Task<ApiResponse<object>> Handle(VerifyEmployeeEmailCommand request)
        {
            var verifyEmailDto = request.VerifyEmailDto;
            if (verifyEmailDto == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest ,message: "Invalid verify email data");
            }
            var otp = await _otpRepository.FindAsync(o =>
                o.OtpCode == verifyEmailDto.Otp &&
                !o.IsVerified &&
                o.ExpiryTime > DateTime.UtcNow);
            if (otp == null)
            {
                return new ApiResponse<object>( System.Net.HttpStatusCode.Conflict ,message: "Invalid OTP or key");
            }
            otp.IsVerified = true;
            _otpRepository.Update(otp.Id, otp);

            var userProfile = await _userProfileRepository.FindAsync(u => u.Id == otp.UserProfileId);
            var existingUser = await _employerRepository.FindAsync(u => u.Id == userProfile.UserID);
            existingUser.Status = true;
            _employerRepository.Update(existingUser.Id, existingUser);
            var access_token = _jwtUtils.GenerateJwtTokenForEmployee(existingUser, string.Empty);
            return new ApiResponse<object>( System.Net.HttpStatusCode.OK, data: new { access_token, existingUser }, message: "Email verified successfully");
        }

    }
}
