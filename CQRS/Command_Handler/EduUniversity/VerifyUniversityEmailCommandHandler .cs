using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using Edu_Block_dev.Authorization;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using AutoMapper;
using Edu_Block_dev.Modal.DTO;
using System.Net;
using Edu_Block_dev.DAL.EF;
using Role = Edu_Block_dev.DAL.EF.Role;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class VerifyUniversityEmailCommandHandler: IRequestHandler<VerifyUniversityEmailCommand, ApiResponse<object>>
    {
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<UniversityUser> _universityUserRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;

        public VerifyUniversityEmailCommandHandler(IRepository<Role> roleRepository ,IRepository<UserProfile> userProfileRepository, IRepository<UniversityUser> universityUserRepository, IRepository<Otp> otpRepository, IRepository<University> universityRepository, IJwtUtils jwtUtils, IMapper mapper)
        {
            _otpRepository = otpRepository;
            _universityUserRepository = universityUserRepository;
            _universityRepository = universityRepository;
            _userProfileRepository = userProfileRepository;
            _roleRepository = roleRepository;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
        }

        public async Task<ApiResponse<object>> Handle(VerifyUniversityEmailCommand request, CancellationToken cancellationToken)
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
                return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound ,message: "Invalid OTP or key");
            }
            otp.IsVerified = true;
            _otpRepository.Update(otp.Id, otp);
            var userProfile = await _userProfileRepository.FindAsync(u => u.Id == otp.UserProfileId);

            Role role = await _roleRepository.FindAsync(u => u.Id == request.VerifyEmailDto.RoleId);
            Role roleInstitution = await _roleRepository.FindAsync(r => r.NormalizedName == "INSTITUTION");

            if (role == null)
            {
                return new ApiResponse<object>(HttpStatusCode.NotFound, data: null, message: "Unable to login");
            }

            if (role.Id == roleInstitution.Id)
            {
                University existingUniversity = await _universityRepository.FindAsync(u => u.Id == userProfile.UserID);
                existingUniversity.Status = true;
                _universityRepository.Update(existingUniversity.Id, existingUniversity);
                var access_token = _jwtUtils.GenerateJwtTokenForUniversity(existingUniversity);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { access_token, UniversityResponseDTO = _mapper.Map<UniversityResponseDTO>(existingUniversity) }, message: "Email verified successfully");

            }
            else
            {
                UniversityUser existingUniversityUser = await _universityUserRepository.FindAsync(u => u.Id == userProfile.UserID);
                existingUniversityUser.Status = true;
                _universityUserRepository.Update(existingUniversityUser.Id, existingUniversityUser);
                var access_token = _jwtUtils.GenerateJwtTokenForUniversityUser(existingUniversityUser);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { access_token, existingUniversityUser }, message: "Email verified successfully");
            }
        }
    }
}