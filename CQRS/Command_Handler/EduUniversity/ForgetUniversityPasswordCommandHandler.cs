using System.Net;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class ForgetUniversityPasswordCommandHandler : IRequestHandler<ForgetUniversityPasswordCommand, ApiResponse<object>>
    {
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<UniversityUser> _universityUserRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly Util _util;
        private readonly IConfiguration _configuration;

        public ForgetUniversityPasswordCommandHandler(IRepository<Role> roleRepository, IRepository<UniversityUser> universityUserRepository, IRepository<UserProfile> userProfileRepository, IRepository<University> universityRepository, IRepository<Otp> otpRepository, Util util, IConfiguration configuration)
        {
            _universityRepository = universityRepository;
            _universityUserRepository = universityUserRepository;
            _otpRepository = otpRepository;
            _userProfileRepository = userProfileRepository;
            _util = util;
            _configuration = configuration;
            _roleRepository = roleRepository;
        }

        public async Task<ApiResponse<object>> Handle(ForgetUniversityPasswordCommand command, CancellationToken cancellationToken)
        {

            Role role = await _roleRepository.FindAsync(u => u.Id == command.EmailRoleGuidDTO.RoleId);
            Role roleInstitution = await _roleRepository.FindAsync(r => r.NormalizedName == "INSTITUTION");

            if (role == null)
            {
                return new ApiResponse<object>(HttpStatusCode.NotFound, data: null, message: "Unable to found role");
            }

           
            if (role.Id == roleInstitution.Id)
            {

                University existingUser = await _universityRepository.FindAsync(u => u.Email == command.EmailRoleGuidDTO.Email);
                if (existingUser == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, data: null, message: "User doesn't exists");
                }
                var existingUserProfile = await _userProfileRepository.FindAsync(u => u.UserID == existingUser.Id);
                if (existingUser == null || existingUserProfile == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, data: null, message: "User doesn't exists");
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
                _util.SendSimpleMessage(subject, command.EmailRoleGuidDTO.Email, content, null, null);
                var responseData = new { key };
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: responseData, message: $"OTP Sent To {command.EmailRoleGuidDTO.Email}");

            }
            else
            {
                UniversityUser existingUniversityUser = await _universityUserRepository.FindAsync(u => u.Email == command.EmailRoleGuidDTO.Email);
                if (existingUniversityUser == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, data: null, message: "User doesn't exists");
                }
                var existingUserProfile = await _userProfileRepository.FindAsync(u => u.UserID == existingUniversityUser.Id);
                if (existingUniversityUser == null || existingUserProfile == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, data: null, message: "User doesn't exists");
                }
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string userSpecificInfo = existingUniversityUser.Id.ToString();
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
                _util.SendSimpleMessage(subject, command.EmailRoleGuidDTO.Email, content, null, null);
                var responseData = new { key };
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: responseData, message: $"OTP Sent To {command.EmailRoleGuidDTO.Email}");
            }
        }
    }
}
