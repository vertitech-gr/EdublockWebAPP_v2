 using System.Net;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.Dock;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUniversity
{
    public class CreateUniversityCommandHandler : IRequestHandler<CreateUniversityCommand, ApiResponse<object>>
    {
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly Util _util;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateUniversityCommandHandler(IRepository<UserRole> userRoleRepository, IRepository<Role> roleRepository, IRepository<University> universityRepository, Util util, IMediator mediator, IMapper mapper, IConfiguration configuration)
        {
            _universityRepository = universityRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _util = util;
            _mediator = mediator;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(CreateUniversityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                UniversityRequestDTO universityRequestDTO = request.UniversityRequestDTO;
                if (universityRequestDTO == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "Invalid university data.");
                }
                University existingUniversity = await _universityRepository.FindAsync(u => u.Email == universityRequestDTO.Email);
                if (existingUniversity != null)
                {
                    return new ApiResponse<object>(HttpStatusCode.Conflict, message: "University with this email already exists.");
                }
                Guid adminId = request.User.UserProfile.Id;
                string universityPassword = _util.GeneratePassword(8);
                University universityEntity = _mapper.Map<University>(request.UniversityRequestDTO);
                universityEntity.UpdatedBy = adminId;
                universityEntity.CreatedBy = adminId;
                universityEntity.universityId = request.UniversityRequestDTO.UniversityId ?? Guid.Empty ;
                universityEntity.Password = BCrypt.Net.BCrypt.HashPassword(universityPassword);
                var addedUniversity = await _universityRepository.AddAsync(universityEntity);
                await _mediator.Send(new UniversityDetailsCommand(request.UniversityRequestDTO, addedUniversity.Id));
                UserRole userRole = _mapper.Map<UserRole>(universityEntity);
                string RoleName = string.Empty;
               
                if (universityRequestDTO.RoleGuid != null && universityRequestDTO.RoleGuid != Guid.Empty)
                {
                   userRole.UserRoleGuid = universityRequestDTO.RoleGuid ?? Guid.Empty;
                    Role role = await _roleRepository.FindAsync(r => r.Id == userRole.UserRoleGuid);
                    RoleName = role.Name;
                }
                else
                {
                    Role role = await _roleRepository.FindAsync( r => r.NormalizedName == "INSTITUTION");
                    userRole.UserRoleGuid = role.Id;
                    RoleName = role.Name;
                }
                _userRoleRepository.Add(userRole);
                UserProfile userProfile = _mapper.Map<UserProfile>(universityEntity);
                await _mediator.Send(new UserProfileCommand(userProfile));
                CreateHandlerResponse createHandlerResponse = await _mediator.Send(new CreateDockHandlerCommand());
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
                var link = _configuration.GetSection("DashboardUrl:University").Value + "/login?role=" + userRole.UserRoleGuid;

                var subject = "Onboarding on EduBlock";
                var content = $"<p>Dear {universityEntity.Name}, We welcome you on Edublock Platform</p>" +
                $"<br>" +
                $"<span> User Id : <strong>{universityEntity.Email}</strong></span>" +
                $"<br>" +
                $"<span> Password : <strong>{universityPassword}</strong></span>" +
                $"<br>" +
                $"<span> Role : <strong>{RoleName}</strong></span>" +
                $"<br>" +
                $"<span>Click on this link : <strong>{link}</strong></span>";

                _util.SendSimpleMessage(subject, universityEntity.Email, content, "Go to Dashboard", link);

                return new ApiResponse<object>( HttpStatusCode.OK, message: "Check email Click on the given link");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>( HttpStatusCode.InternalServerError, error: ex.Message, message: "University creation unsuccessfull");

            }
        }
   }
}
