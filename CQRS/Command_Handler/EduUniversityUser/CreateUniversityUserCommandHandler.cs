using System.Net;
using Aspose.Pdf.Drawing;
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
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUniversity
{
    public class CreateUniversityUserCommandHandler: IRequestHandler<CreateUniversityUserCommand, ApiResponse<object>>
    {
        private readonly IRepository<UniversityUser> _universityUserRepository;
        private readonly IRepository<UniversityDepartmentUser> _universityDepartmentUserRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly Util _util;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly EduBlockDataContext _context;

        public CreateUniversityUserCommandHandler(IRepository<UniversityUser> universityUserRepository, IRepository<UniversityDepartmentUser> universityDepartmentUserRepository, IRepository<UserRole> userRoleRepository, IRepository<Role> roleRepository, IRepository<University> universityRepository, Util util, IMediator mediator, IMapper mapper, IConfiguration configuration, EduBlockDataContext context)
        {
            _universityUserRepository = universityUserRepository;
            _universityDepartmentUserRepository = universityDepartmentUserRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _util = util;
            _mediator = mediator;
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
        }

        public async Task<ApiResponse<object>> Handle(CreateUniversityUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                UniversityUserRequestDTO universityUserRequestDTO = request.UniversityUserRequestDTO;
                if (universityUserRequestDTO == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "Invalid university user data.");
                }
                UniversityUser existingUniversityUser = await _universityUserRepository.FindAsync(u => u.Email == universityUserRequestDTO.Email);

                Role sectretriatRole = await _roleRepository.FindAsync(r => r.NormalizedName == Authorization.Role.SECRETARIAT.ToString());

                if(universityUserRequestDTO.RoleId == sectretriatRole.Id)
                {
                    var testUsers = _context.UniversityUsers.Where(u => u.RoleId == sectretriatRole.Id).Join(_context.UniversityDepartmentUsers, uu => uu.Id, udu => udu.UniversityUserId, (uu, udu) => new
                    {
                        UniversityUser = uu,
                        UniversityDepartmentUser = udu
                    }).Where(t => t.UniversityDepartmentUser.DepartmentId == universityUserRequestDTO.DepartmentId && t.UniversityDepartmentUser.UniversityId == universityUserRequestDTO.UniversityId).ToList();

                    if(testUsers.Count() > 1)
                    {
                        return new ApiResponse<object>(System.Net.HttpStatusCode.AlreadyReported, message: "Secretriet role is already assigned.");

                    }
                }

                if (existingUniversityUser != null)
                {
                    UniversityDepartmentUser existingUniversityDepartmentUser = _mapper.Map<UniversityDepartmentUser>(request.UniversityUserRequestDTO);
                    existingUniversityDepartmentUser.UniversityUserId = existingUniversityUser.Id;
                    await _universityDepartmentUserRepository.AddAsync(existingUniversityDepartmentUser);
                    return new ApiResponse<object>(HttpStatusCode.OK, message: "Added Another Department Successfully");
                }
                string universityPassword = _util.GeneratePassword(8);
                UniversityUser universityUserEntity = _mapper.Map<UniversityUser>(request.UniversityUserRequestDTO);
                universityUserEntity.Password = BCrypt.Net.BCrypt.HashPassword(universityPassword);
                var addedUniversityUser = await _universityUserRepository.AddAsync(universityUserEntity);
                UniversityDepartmentUser universityDepartmentUser = _mapper.Map<UniversityDepartmentUser>(request.UniversityUserRequestDTO);
                universityDepartmentUser.UniversityUserId = addedUniversityUser.Id;
                var addedUniversityDeparmentUser = await _universityDepartmentUserRepository.AddAsync(universityDepartmentUser);
                UserRole userRole = _mapper.Map<UserRole>(universityUserEntity);
                Role role = await _roleRepository.FindAsync(r => r.Id == userRole.UserRoleGuid);
                _userRoleRepository.Add(userRole);
                UserProfile userProfile = _mapper.Map<UserProfile>(universityUserEntity);
                await _mediator.Send(new UserProfileCommand(userProfile));
                CreateHandlerResponse createHandlerResponse = await _mediator.Send(new CreateDockHandlerCommand());
                IssuerDTO issuerDTO = new IssuerDTO()
                {
                     Name = universityUserRequestDTO.Name,
                };
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
                var content = $"<p>Dear {universityUserEntity.Name}, We welcome you on Edublock Platform</p>" +
                $"<br>" +
                $"<span> User Id : <strong>{universityUserEntity.Email}</strong></span>" +
                $"<br>" +
                $"<span> Password : <strong>{universityPassword}</strong></span>" +
                $"<br>" +
                $"<span> Role : <strong>{role.Name}</strong></span>" +
                $"<br>" +
                $"<span>Click on this link : <strong>{link}</strong></span>";
                _util.SendSimpleMessage(subject, universityUserEntity.Email, content, "Go to dashboard", link );
                return new ApiResponse<object>( HttpStatusCode.OK, message: "Check email Click on the given link");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>( HttpStatusCode.InternalServerError, error: ex.Message, message: "University creation unsuccessfull");

            }
        }
   }
}
