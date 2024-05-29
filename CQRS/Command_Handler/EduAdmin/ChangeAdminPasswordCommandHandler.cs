using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduUser;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class ChangeAdminPasswordCommandHandler: IRequestHandler<ChangeAdminPasswordCommand, ApiResponse<object>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;

        public ChangeAdminPasswordCommandHandler(IRepository<UserProfile> userProfileRepository,IRepository<User> userRepository, IRepository<Otp> otpRepository)
        {
            _otpRepository = otpRepository;
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<ApiResponse<object>> Handle(ChangeAdminPasswordCommand command, CancellationToken cancellationToken)
        {
            var changePasswordDto = command.ChangePasswordDto;
            if (changePasswordDto == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest,message: "Invalid change password data");
            }
            var otp = await _otpRepository.FindAsync(o => o.OtpCode == changePasswordDto.Otp && o.key == changePasswordDto.Key && !o.IsVerified && o.ExpiryTime > DateTime.UtcNow);
            if (otp == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, message: "Invalid OTP or key");
            }
            otp.IsVerified = true;
            _otpRepository.Update(otp.Id, otp);
            var userProfile = await _userProfileRepository.FindAsync(u => u.Id == otp.UserProfileId);
            var existingUser = await _userRepository.FindAsync(u => u.Id == userProfile.UserID);
            existingUser.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            _userRepository.Update(existingUser.Id, existingUser);
            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, message: "Password changed successfully");
        }
    }
}
