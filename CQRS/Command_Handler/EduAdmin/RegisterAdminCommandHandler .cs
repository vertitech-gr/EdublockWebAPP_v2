using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using EduBlock.Model.DTO;
using MediatR;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.CQRS.Commands;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.DAL.EF;
using AutoMapper;
using Edu_Block_dev.Modal.Dock;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, ApiResponse<object>>
    {
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<Edu_Block.DAL.EF.Admin> _adminRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly Util _util;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public RegisterAdminCommandHandler(IMapper mapper, IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository, IRepository<Edu_Block.DAL.EF.Admin> adminRepository, IRepository<Otp> otpRepository, Util util, IMediator mediator, IConfiguration configuration)
        {
            _adminRepository = adminRepository;
            _userRoleRepository = userRoleRepository;
            _otpRepository = otpRepository;
            _roleRepository = roleRepository;
            _util = util;
            _mediator = mediator;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserDto == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest,message: "Invalid admin data");
                }
                Edu_Block.DAL.EF.Admin existingAdmin = await _adminRepository.FindAsync(u => u.Email == request.UserDto.Email);
                if (existingAdmin != null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.Conflict,message: "Admin with this email already exists");
                }
                Edu_Block.DAL.EF.Admin adminEntity = _mapper.Map<Edu_Block.DAL.EF.Admin>(request.UserDto);
                _adminRepository.Add(adminEntity);
                Role role = await _roleRepository.FindAsync(r => r.NormalizedName == Authorization.Role.SECRETARIAT.ToString());

                UserRole userRole = _mapper.Map<UserRole>(adminEntity);
                userRole.UserRoleGuid = role.Id;
                _userRoleRepository.Add(userRole);

                UserProfile userProfile = _mapper.Map<UserProfile>(adminEntity);
                await _mediator.Send(new UserProfileCommand(userProfile));

                Otp otp = _mapper.Map<Otp>(userProfile);
                otp.key = _util.GetDataToHash(adminEntity.Id);
                otp.OtpCode = _util.GenerateOtp();
                otp.ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value));
                _otpRepository.Add(otp);

                CreateHandlerResponse createHandlerResponse = await _mediator.Send(new CreateDockHandlerCommand());

                DockIoDID dockDID = new DockIoDID()
                {
                    DID = createHandlerResponse.Did,
                    UserProfileId = userProfile.Id,
                    Credentials = string.Empty,
                    Password = _configuration.GetSection("Dock:Password").Value
                };

                DockIoDID CreateDockDIDResult = await _mediator.Send(new CertificateDIDCommand(dockDID));
                var subject = "Verify Registration --------- EduBlock";
                var content = $"<p>Dear Admin, your OTP is only valid for 15 minutes</p>" +
                              $"<br>" +
                              $"<span>OTP : <strong>{otp.OtpCode}</strong></span>";
                _util.SendSimpleMessage(subject, adminEntity.Email, content, null, null);

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK,data: new { otp.key }, message: "Verify email with OTP");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError,message: "Admin registration unsuccessfull");

            }

        }
    }
}
