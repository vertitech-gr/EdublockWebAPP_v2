using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduEmployer;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class EmployeeChangePasswordCommandHandler : IRequestHandler<EmployeeChangePasswordCommand, ApiResponse<object>>
    {
        private readonly IRepository<Employer> _employerRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;

        public EmployeeChangePasswordCommandHandler(IRepository<Employer> employerRepository, IRepository<UserProfile> userProfileRepository,IRepository<User> userRepository, IRepository<Otp> otpRepository)
        {
            _otpRepository = otpRepository;
            _employerRepository = employerRepository;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<ApiResponse<object>> Handle(EmployeeChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var changePasswordDto = command.ChangePasswordDTO;
            if (changePasswordDto == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: "Password changed successfully");
            }
            var otp = await _otpRepository.FindAsync(o => o.OtpCode == changePasswordDto.Otp && o.key == changePasswordDto.Key && !o.IsVerified && o.ExpiryTime > DateTime.UtcNow);
            if (otp == null)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: "Invalid OTP or key");
            }
            otp.IsVerified = true;
            _otpRepository.Update(otp.Id, otp);
            var userProfile = await _userProfileRepository.FindAsync(u => u.Id == otp.UserProfileId);
            var existingUser = await _employerRepository.FindAsync(u => u.Id == userProfile.UserID);
            existingUser.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            _employerRepository.Update(existingUser.Id, existingUser);
            return new ApiResponse<object>( System.Net.HttpStatusCode.OK, message: "Password changed successfully");
        }
    }
}