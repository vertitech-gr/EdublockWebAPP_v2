using Edu_Block.DAL;
using EduBlock.Model.DTO;
using MediatR;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block_dev.Modal.DTO;
using AutoMapper;
using System.Net;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduUser;
using Role = Edu_Block_dev.DAL.EF.Role;
using Edu_Block.DAL.EF;
using System.Threading.Tasks;
using System.Linq;



namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class UniversityLoginHandler : IRequestHandler<UniversityLoginCommand, ApiResponse<object>>
    {
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<UniversityUser> _universityUserRepository;
        private readonly IRepository<UniversityDepartmentUser> _universityDepartmentUserRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<DockIoDID> _dockRepository;
        private readonly EduBlockDataContext _context;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public UniversityLoginHandler(EduBlockDataContext context, IMediator mediator, IRepository<Role> roleRepository, IRepository<UniversityDepartmentUser> universityDepartmentUserRepository, IRepository<UniversityUser> universityUserRepository, IRepository<UserProfile> userProfileRepository, IRepository<University> universityRepository, IRepository<DockIoDID> dockRepository, IJwtUtils jwtUtils, IMapper mapper)
        {
            _universityRepository = universityRepository;
            _userProfileRepository = userProfileRepository;
            _universityUserRepository = universityUserRepository;
            _universityDepartmentUserRepository = universityDepartmentUserRepository;
            _context = context;
            _dockRepository = dockRepository;
            _roleRepository = roleRepository;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponse<object>> Handle(UniversityLoginCommand request, CancellationToken cancellationToken)
        {

            Role role = await _roleRepository.FindAsync( u => u.Id == request.UniversityLoginDTO.RoleId);
            Role roleInstitution = await _roleRepository.FindAsync(r => r.NormalizedName == "INSTITUTION");

            if (role == null)
            {
                return new ApiResponse<object>(HttpStatusCode.NotFound, data: null, message: "Unable to login");
            }

            if(role.Id == roleInstitution.Id)
            {
                var existingUniversity = await _universityRepository.FindAsync(u => u.Email == request.UniversityLoginDTO.Email);
                if (existingUniversity == null || !BCrypt.Net.BCrypt.Verify(request.UniversityLoginDTO.Password, existingUniversity.Password))
                {
                    return new ApiResponse<object>(HttpStatusCode.Unauthorized, data: new { LoginStatus = LoginStatus.Invalid }, message: "Invalid email or password");
                }
                RolesAndPermissionDTO rolesAndPermissionDTO = new RolesAndPermissionDTO();
                UserProfile userProfile = await _userProfileRepository.FindAsync(u => u.UserID == existingUniversity.Id);
                DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == userProfile.Id);
                UserRole userRole = await _mediator.Send(new GetUserRoleQuery(userProfile.Id));
                if (userRole != null)
                {
                    rolesAndPermissionDTO = await _mediator.Send(new GetRolesAndPermissionQuery(userRole));
                }
                var access_token = _jwtUtils.GenerateJwtTokenForUniversity(existingUniversity);
                return new ApiResponse<object>(HttpStatusCode.OK, data: new { LoginStatus = LoginStatus.Completed, UniversityResponseDTO = _mapper.Map<UniversityResponseDTO>(existingUniversity), rolesAndPermissionDTO, access_token, did = dock.DID, userProfile }, message: "University login successfully");
            }
            else
            {
                var existingUniversityUser = await _universityUserRepository.FindAsync(u => u.Email == request.UniversityLoginDTO.Email);
                if (existingUniversityUser == null || !BCrypt.Net.BCrypt.Verify(request.UniversityLoginDTO.Password, existingUniversityUser.Password))
                {
                    return new ApiResponse<object>(HttpStatusCode.Unauthorized, data: new { LoginStatus = LoginStatus.Invalid }, message: "Invalid email or password");
                }
                RolesAndPermissionDTO rolesAndPermissionDTO = new RolesAndPermissionDTO();
                UserProfile userProfile = await _userProfileRepository.FindAsync(u => u.UserID == existingUniversityUser.Id);
                DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == userProfile.Id);
                UserRole userRole = await _mediator.Send(new GetUserRoleQuery(existingUniversityUser.Id));
                if (userRole != null)
                {
                    rolesAndPermissionDTO = await _mediator.Send(new GetRolesAndPermissionQuery(userRole));
                }
                var access_token = _jwtUtils.GenerateJwtTokenForUniversityUser(existingUniversityUser);
               
                var university =
                    _context.UniversityUsers.Where( uu => uu.Id == existingUniversityUser.Id)
                    .GroupJoin(_context.UniversityDepartmentUsers, uu => uu.Id, udu => udu.UniversityUserId, (uu, udu) => new
                    {
                        UniversityUsers = uu,
                        UniversityDepartmentUsers = udu.FirstOrDefault()
                    }).Join(_context.Universities,
                         uudu => uudu.UniversityDepartmentUsers.UniversityId,
                         u => u.Id,
                         (ud, u) => u).FirstOrDefault();

                UserProfile universityUserProfile = await _userProfileRepository.FindAsync(u => u.UserID == university.Id);
                DockIoDID universityDock = await _dockRepository.FindAsync(u => u.UserProfileId == universityUserProfile.Id);


                UniversityResponseDTO universityResponseDTO = new UniversityResponseDTO()
                {
                     Id = university.Id,
                     Active = university.Active,
                     Status = university.Status,
                     Email = university.Email,
                     did = universityDock.DID
                };

                return new ApiResponse<object>(HttpStatusCode.OK, data: new { LoginStatus = LoginStatus.Completed, universityResponseDTO, rolesAndPermissionDTO, universityUser = existingUniversityUser, access_token, did = dock.DID, userProfile }, message: "University user login successfully");

            }
        }
    }
}