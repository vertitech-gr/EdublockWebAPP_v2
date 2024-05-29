using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Role = Edu_Block_dev.Authorization.Role;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, ApiResponse<object>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Employer> _employerRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly Util _util;

        public ResendOtpCommandHandler(IRepository<Employer> employerRepository,IRepository<UserProfile> userProfileRepository,IRepository<User> userRepository, IRepository<Otp> otpRepository, Util util)
        {
            _userProfileRepository = userProfileRepository;
            _userRepository = userRepository;
            _util = util;
            _otpRepository = otpRepository;
            _employerRepository = employerRepository;
        }

        public async Task<ApiResponse<object>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            try {

                Employer employer = new Employer();
                User user = new User();
                CommanUser commanUser = new CommanUser();

                switch (request.Type)
                {
                    case Role.EMPLOYER:
                        employer = await _employerRepository.FindAsync(u => u.Email == request.Email);
                        commanUser = new CommanUser()
                        {
                            Id = employer.Id,
                            Name = employer.Name,
                            CreatedAt = employer.CreatedAt,
                            Email = employer.Email,
                            Password = employer.Password,
                            Status = employer.Status,
                            UpdatedAt = employer.UpdatedAt
                        };
                        break;
                    default:
                        user = await _userRepository.FindAsync(u => u.Email == request.Email);
                        commanUser = new CommanUser()
                        {
                            Id = user.Id,
                            Name = user.Name,
                            CreatedAt = user.CreatedAt,
                            Email = user.Email,
                            Password = user.Password,
                            Status = user.Status,
                            UpdatedAt = user.UpdatedAt
                        };
                        break;
                }
             
                if (commanUser == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, message: "User not found");
                }

                var userProfile = await _userProfileRepository.FindAsync(u => u.UserID == commanUser.Id);
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string userSpecificInfo = user.Id.ToString();
                string dataToHash = $"{timestamp}-{userSpecificInfo}";
                var key = _util.ComputeSha256Hash(dataToHash);
                Otp otp = new Otp
                {
                    key = key,
                    OtpCode = _util.GenerateOtp(),
                    UserProfileId = userProfile.Id,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(15),
                    OtpType = OtpType.REGISTER
                };
                _otpRepository.Add(otp);
                var subject = "Resend Verification OTP --------- EduBlock";
                var content = $"<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                              $"<br>" +
                              $"<span>OTP : <strong>{otp.OtpCode}</strong></span>";
                _util.SendSimpleMessage(subject, request.Email, content, null, null);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { key }, message: "OTP Resent for email verification");

            }
            catch (Exception ex) {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, data: null, message: "Unable to send OTP");

            }
        }
    }
}