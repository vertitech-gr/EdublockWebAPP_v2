using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.CQRS.Commands;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.CQRS.Commands.EduEmployer;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class RegisterEmployeeCommandHandler : IRequestHandler<RegisterEmployeeCommand, ApiResponse<object>>
    {
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<Employer> _employeeRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly Util _util;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public RegisterEmployeeCommandHandler(IRepository<Role> roleRepository,IRepository<UserRole> userRoleRepository, IRepository<Employer> employeeRepository, IRepository<Otp> otpRepository, Util util, IMediator mediator, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _otpRepository = otpRepository;
            _util = util;
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
        {
            try {
                var employerDTO = request.employerDTO;

                if (employerDTO == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest ,message: "Invalid employee data");
                }

                var existingUser = await _employeeRepository.FindAsync(u => u.Email == employerDTO.Email);

                if (existingUser != null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.Conflict , message: "Employee with this email already exists");
                }

                var employerEntity = new Employer
                {
                    Name = request.employerDTO.Name,
                    Email = request.employerDTO.Email,
                    Address = request.employerDTO.Address,
                    Industry = request.employerDTO.Industry,
                    SpecificIndustry = request.employerDTO.SpecificIndustry,
                    CountryCode = request.employerDTO.CountryCode,
                    PhoneNumber = request.employerDTO.PhoneNumber
                };

                employerEntity.Id = Guid.NewGuid();
                employerEntity.Password = BCrypt.Net.BCrypt.HashPassword(request.employerDTO.Password);
                var addedEmployee = await _employeeRepository.AddAsync(employerEntity);
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string userSpecificInfo = addedEmployee.Id.ToString();
                string dataToHash = $"{timestamp}-{userSpecificInfo}";
                var key = _util.ComputeSha256Hash(dataToHash);

                Role role =  await _roleRepository.FindAsync( r => r.NormalizedName == Authorization.Role.EMPLOYER.ToString());

                UserRole userRole = new UserRole();
                userRole.UserRoleId = Authorization.Role.EMPLOYER;
                userRole.UserId = employerEntity.Id;
                userRole.UserRoleGuid = role.Id;
                _userRoleRepository.Add(userRole);

                UserProfile userProfile = new UserProfile()
                {
                    UserID = addedEmployee.Id,
                    Name = employerEntity.Name,
                    Description = employerEntity.Name,
                };

                var UserProfileCommandResult = await _mediator.Send(new UserProfileCommand(userProfile));
                var UserProfileQueryResult = await _mediator.Send(new GetUserProfileQuery(userProfile));

                Otp otp = new Otp();
                otp.key = key;
                otp.OtpCode = _util.GenerateOtp();
                otp.UserProfileId = UserProfileQueryResult.Id;
                otp.ExpiryTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("ShareConfig:ExpiryTime").Value));
                otp.OtpType = OtpType.REGISTER;
                _otpRepository.Add(otp);

                var CreateDockHandlerRresult = await _mediator.Send(new CreateDockHandlerCommand());

                DockIoDID dockDID = new DockIoDID()
                {
                    DID = CreateDockHandlerRresult.Did,
                    UserProfileId = UserProfileQueryResult.Id,
                    Credentials = "",
                    Password = _configuration.GetSection("Dock:Password").Value

                };
                var CreateDockDIDResult = await _mediator.Send(new CertificateDIDCommand(dockDID));


                var subject = "Verify Registration --------- EduBlock";
                var content = $"<p>Dear Customer, your OTP is only valid for 15 minutes</p>" +
                              $"<br>" +
                              $"<span>OTP : <strong>{otp.OtpCode}</strong></span>";

                _util.SendSimpleMessage(subject, employerEntity.Email, content, null, null);

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK,data: new { key }, message: "Verify email with OTP");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError ,message: "Employer registration unsuccessfull");

            }

        }
    }
}
