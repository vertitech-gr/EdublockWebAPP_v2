using System.Net;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands;
using Edu_Block_dev.CQRS.Commands.EduDepartment;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.Dock;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduDepartment
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, ApiResponse<object>>
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly Util _util;
        private readonly EduBlockDataContext _context;

        public CreateDepartmentCommandHandler(IRepository<UserRole> userRoleRepository, IRepository<Role> roleRepository, EduBlockDataContext context, IConfiguration configuration, Util util,IRepository<Department> departmentRepository, IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _departmentRepository = departmentRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _mapper = mapper;
            _util = util;
            _context = context;
        }

        public async Task<ApiResponse<object>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.DepartmentDto == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "Invalid Department data.");
                }

                //Department existingDepartment = await _departmentRepository.FindAsync( d => d.Email == request.DepartmentDto.Email);
                //if(existingDepartment != null)
                //{
                //    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, data: null, message: "Department with this email already exists.");
                //}

                if(request.User != null)
                {
                    request.DepartmentDto.UniversityID = request.User.Id;
                }
                Department existingDepartment = await _departmentRepository.FindAsync(d => d.Type == request.DepartmentDto.Type && d.UniversityID == request.DepartmentDto.UniversityID);
                if(existingDepartment != null)
                {
                    return new ApiResponse<object>(HttpStatusCode.BadRequest, message: "Department Already Exist");
                }
                Department department = _mapper.Map<Department>(request.DepartmentDto);
                department.UpdatedBy = request.DepartmentDto.UniversityID;
                department.CreatedBy = request.DepartmentDto.UniversityID;
                department.Password = _util.GeneratePassword(8);
                var addedDepartment = await _departmentRepository.AddAsync(department);

                Role role =  await _roleRepository.FindAsync(r => r.NormalizedName == Authorization.Role.SECRETARIAT.ToString());

                UserRole userRole = _mapper.Map<UserRole>(department);
                userRole.UserRoleGuid = role.Id;
                await _userRoleRepository.AddAsync(userRole);

                UserProfile userProfile = _mapper.Map<UserProfile>(department);
                await _mediator.Send(new UserProfileCommand(userProfile));

                CreateHandlerResponse createHandlerResponse = await _mediator.Send(new CreateDockHandlerCommand());
                if (createHandlerResponse == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "University creation unsuccessfull");
                }

                DockIoDID dockDID = new DockIoDID()
                {
                    DID = createHandlerResponse.Did,
                    UserProfileId = userProfile.Id,
                    Credentials = string.Empty,
                    Password = _configuration.GetSection("Dock:Password").Value
                };
                var CreateDockDIDResult = await _mediator.Send(new CertificateDIDCommand(dockDID));

                var subject = "Onboarding on EduBlock";
                var content = $"<p>Dear {department.Name}, We welcome you on Edublock Platform</p>" +
                $"<br>" +
                $"<span> User Id : <strong>{department.Email}</strong></span>" +
                $"<br>" +
                $"<span> Password : <strong>{department}</strong></span>" +
                $"<br>" +
                $"<span>Click on this link : <strong>{_configuration.GetSection("DashboardUrl:University").Value + "/login"}</strong></span>";

                //_util.SendSimpleMessage(subject, department.Email, content, null, null );

                if (addedDepartment == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Department creation unsuccessfull");
                }

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: department, message: "Department creation successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, data: null , message: ex.Message);
            }
        }
    }
}