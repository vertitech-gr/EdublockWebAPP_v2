using System.Net;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.Dock;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUniversity
{
	public class RegisterUniversityCommandHandler: IRequestHandler<RegisterUniversityCommand, ApiResponse<object>>
    {
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly Util _util;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public RegisterUniversityCommandHandler(IRepository<UserRole> userRoleRepository, IRepository<University> universityRepository, IRepository<Otp> otpRepository, Util util, IMediator mediator, IMapper mapper, IConfiguration configuration)
        {
            _universityRepository = universityRepository;
            _userRoleRepository = userRoleRepository;
            _otpRepository = otpRepository;
            _util = util;
            _mediator = mediator;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(RegisterUniversityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                UniversityRequestDTO UniversityRequestDTO = request.UniversityRequestDTO;
                if (UniversityRequestDTO == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.BadRequest, message: "Invalid university data.");
                }
                University existingUniversity = await _universityRepository.FindAsync(u => u.Email == UniversityRequestDTO.Email);
                if (existingUniversity != null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound,message: "University with this email already exists.");
                }
                University universityEntity = _mapper.Map<University>(request.UniversityRequestDTO);
                var addedUniversity = _universityRepository.Add(universityEntity);
                if(addedUniversity == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Employer registration unsuccessfull");

                }
                await _mediator.Send(new UniversityDetailsCommand(request.UniversityRequestDTO, addedUniversity.Id));

                UserRole userRole = _mapper.Map<UserRole>(universityEntity);
                _userRoleRepository.Add(userRole);

                UserProfile userProfile = _mapper.Map<UserProfile>(universityEntity);
                await _mediator.Send(new UserProfileCommand(userProfile));

                Otp otp = new Otp();
                otp.key = _util.GetDataToHash(addedUniversity.Id);
                otp.OtpCode = _util.GenerateOtp();
                otp.UserProfileId = userProfile.Id;
                otp.ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value));
                otp.OtpType = OtpType.REGISTER;
                _otpRepository.Add(otp);

                CreateHandlerResponse createHandlerResponse = await _mediator.Send(new CreateDockHandlerCommand());
                if(createHandlerResponse == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Employer registration unsuccessfull");
                }

                IssuerDTO issuerDTO = _mapper.Map<IssuerDTO>(request.UniversityRequestDTO);
                await _mediator.Send(new IssuerProfileCommand(createHandlerResponse, issuerDTO));

                DockIoDID dockDID = new DockIoDID()
                {
                    DID = createHandlerResponse.Did,
                    UserProfileId = userProfile.Id,
                    Credentials = string.Empty,
                    Password = _configuration.GetSection("Dock:Password").Value
                };
                var CreateDockDIDResult = await _mediator.Send(new CertificateDIDCommand(dockDID));
                var subject = "Verify Registration --------- EduBlock";
                var content = $"<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                              $"<br>" +
                              $"<span>OTP : <strong>{otp.OtpCode}</strong></span>";

                _util.SendSimpleMessage(subject, universityEntity.Email, content, null, null);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK,data: new { otp.key }, message: "Verify email with OTP");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Employer registration unsuccessfull");

            }
        }
    }
}