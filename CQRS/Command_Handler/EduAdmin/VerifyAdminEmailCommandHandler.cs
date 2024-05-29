using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EduUser;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class VerifyAdminEmailCommandHandler: IRequestHandler<VerifyAdminEmailCommand, ApiResponse<object>>
    {
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<Edu_Block.DAL.EF.Admin> _adminRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IJwtUtils _jwtUtils;

        public VerifyAdminEmailCommandHandler(IRepository<UserProfile> userProfileRepository, IRepository<Otp> otpRepository, IRepository<Edu_Block.DAL.EF.Admin> adminRepository,IJwtUtils jwtUtils)
        {
            _otpRepository = otpRepository;
            _adminRepository = adminRepository;
            _userProfileRepository = userProfileRepository;
            _jwtUtils = jwtUtils;
        }

        public async Task<ApiResponse<object>> Handle(VerifyAdminEmailCommand request, CancellationToken cancellationToken)
        {
            var verifyEmailDto = request.VerifyEmailDto;
            if (verifyEmailDto == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest,message: "Invalid verify email data");
            }
            var otp = await _otpRepository.FindAsync(o =>
                o.OtpCode == verifyEmailDto.Otp &&
                !o.IsVerified &&
                o.ExpiryTime > DateTime.UtcNow);
            if (otp == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError,message: "Invalid OTP or key");
            }
            otp.IsVerified = true;
            _otpRepository.Update(otp.Id, otp);
            var userProfile = await _userProfileRepository.FindAsync(u => u.Id == otp.UserProfileId);
            var existingAdmin = await _adminRepository.FindAsync(u => u.Id == userProfile.UserID);
            existingAdmin.Status = true;
            _adminRepository.Update(existingAdmin.Id, existingAdmin);
            var access_token = _jwtUtils.GenerateJwtTokenForAdmin(existingAdmin);
            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { access_token, existingAdmin }, message: "Email verified successfully");
        }
    }
}