using System.Net;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class ResendUniversityOtpCommandHandler : IRequestHandler<ResendUniversityOtpCommand, ApiResponse<object>>
    {
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<UniversityUser> _universityUserRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly Util _util;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Role> _roleRepository;


        public ResendUniversityOtpCommandHandler(IMapper mapper, IRepository<UniversityUser> universityUserRepository, IRepository<UserProfile> userProfileRepository, IRepository<University> universityRepository, IRepository<Otp> otpRepository, Util util, IConfiguration configuration, IRepository<Role> roleRepository)
        {
            _userProfileRepository = userProfileRepository;
            _universityUserRepository = universityUserRepository;
            _universityRepository = universityRepository;
            _util = util;
            _otpRepository = otpRepository;
            _mapper = mapper;
            _configuration = configuration;
            _roleRepository = roleRepository;
        }

        public async Task<ApiResponse<object>> Handle(ResendUniversityOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Role role = await _roleRepository.FindAsync(u => u.Id == request.RoleId);
                Role roleInstitution = await _roleRepository.FindAsync(r => r.NormalizedName == "INSTITUTION");

                if (role == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.NotFound, data: null, message: "Unable to login");
                }

                if (role.Id == roleInstitution.Id)
                {
                    University university = await _universityRepository.FindAsync(u => u.Email == request.Email);
                    if (university == null)
                    {
                        return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, message: "University doesn't exists with us.");

                    }
                    UserProfile userProfile = await _userProfileRepository.FindAsync(u => u.UserID == university.Id);

                    Otp otp = _mapper.Map<Otp>(userProfile);
                    otp.key = _util.GetDataToHash(university.Id);
                    otp.OtpCode = _util.GenerateOtp();
                    otp.ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value));
                    _otpRepository.Add(otp);
                    var subject = "Resend Verification OTP --------- EduBlock";
                    var content = $"<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                                  $"<br>" +
                                  $"<span>OTP : <strong>{otp.OtpCode}</strong></span>";
                    _util.SendSimpleMessage(subject, university.Email, content, null, null);
                    return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { otp.key }, message: "OTP Resent for email verification");

                }
                else
                {
                    UniversityUser universityUser = await _universityUserRepository.FindAsync(u => u.Email == request.Email);
                    if (universityUser == null)
                    {
                        return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, message: "University user doesn't exists with us.");

                    }
                    UserProfile userProfile = await _userProfileRepository.FindAsync(u => u.UserID == universityUser.Id);

                    Otp otp = _mapper.Map<Otp>(userProfile);
                    otp.key = _util.GetDataToHash(universityUser.Id);
                    otp.OtpCode = _util.GenerateOtp();
                    otp.ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value));
                    _otpRepository.Add(otp);
                    var subject = "Resend Verification OTP --------- EduBlock";
                    var content = $"<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                                  $"<br>" +
                                  $"<span>OTP : <strong>{otp.OtpCode}</strong></span>";
                    _util.SendSimpleMessage(subject, universityUser.Email, content, null, null);
                    return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { otp.key }, message: "OTP Resent for email verification");


                }




            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError ,message: "Unable to send OTP for email verification");

            }
        }
    }
}