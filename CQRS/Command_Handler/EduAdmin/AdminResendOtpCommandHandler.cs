using System.Net;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduUser;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class AdminResendOtpCommandHandler : IRequestHandler<ResendAdminOtpCommand, ApiResponse<object>>
    {
        private readonly IRepository<Admin> _adminRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly Util _util;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AdminResendOtpCommandHandler(IMapper mapper, IRepository<UserProfile> userProfileRepository, IRepository<Admin> adminRepository, IRepository<Otp> otpRepository, Util util, IConfiguration configuration)
        {
            _userProfileRepository = userProfileRepository;
            _adminRepository = adminRepository;
            _util = util;
            _otpRepository = otpRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(ResendAdminOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Admin admin = await _adminRepository.FindAsync(u => u.Email == request.Email);
                //test
                if(admin == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.NotFound,message: "Admin doesn't exists with us.");

                }
                UserProfile userProfile = await _userProfileRepository.FindAsync(u => u.UserID == admin.Id);
               
                Otp otp = _mapper.Map<Otp>(userProfile);
                otp.key = _util.GetDataToHash(admin.Id);
                otp.OtpCode = _util.GenerateOtp();
                otp.ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value));
                _otpRepository.Add(otp);
                var subject = "Resend Verification OTP --------- EduBlock";
                var content = $"<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                              $"<br>" +
                              $"<span>OTP : <strong>{otp.OtpCode}</strong></span>";
                _util.SendSimpleMessage(subject, admin.Email, content, null, null);
                return new ApiResponse<object>(HttpStatusCode.OK, data: new { otp.key }, message: "OTP Resent for email verification");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Unable to send OTP for email verification");

            }
        }
    }
}