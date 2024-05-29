using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using EduBlock.Model.DTO;
using MediatR;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EduUser;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class VerifyEmailCommandHandler
    {
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;

        private readonly Util _util;
        private readonly IJwtUtils _jwtUtils;

        public VerifyEmailCommandHandler(IRepository<UserProfile> userProfileRepository,IRepository<Otp> otpRepository, IRepository<User> userRepository, Util util, IJwtUtils jwtUtils)
        {
            _otpRepository = otpRepository;
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _util = util;
            _jwtUtils = jwtUtils;
        }

        public async Task<ApiResponse<object>> Handle(VerifyEmailCommand request)
        {
            var verifyEmailDto = request.VerifyEmailDto;

            if (verifyEmailDto == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "Invalid verify email data");
            }

            var otp = await _otpRepository.FindAsync(o =>
                o.OtpCode == verifyEmailDto.Otp &&
                !o.IsVerified &&
                o.ExpiryTime > DateTime.UtcNow);

            if (otp == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound , message: "Invalid OTP or key");
            }

            // Mark the OTP as verified
            otp.IsVerified = true;
            _otpRepository.Update(otp.Id, otp);

            var userProfile = await _userProfileRepository.FindAsync(u => u.Id == otp.UserProfileId);

            var existingUser = await _userRepository.FindAsync(u => u.Id == userProfile.UserID);

            // Update the user's email verification status
            existingUser.Status = true;
            _userRepository.Update(existingUser.Id, existingUser);

            var access_token = _jwtUtils.GenerateJwtToken(existingUser);
            return new ApiResponse<object>(System.Net.HttpStatusCode.OK ,data: new { access_token }, message: "Email verified successfully");
        }

    }
}
