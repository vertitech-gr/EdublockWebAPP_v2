using System.Net;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands;
using Edu_Block_dev.CQRS.Commands.EduCertificate;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.Dock;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, ApiResponse<object>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<Certificate> _certificateRepository;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public CreateStudentCommandHandler(IRepository<UserProfile> userProfileRepository, IRepository<Role> roleRepository, IRepository<Certificate> certificateRepository, IRepository<Department> departmentRepository, IConfiguration configuration, IRepository<UserRole> userRoleRepository, Util util,IRepository<User> userRepository, IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _departmentRepository = departmentRepository;
            _certificateRepository = certificateRepository;
            _userProfileRepository = userProfileRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _mapper = mapper;
            _util = util;
        }

        public async Task<ApiResponse<object>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                StudentRequestDTO studentRequestDTO = new StudentRequestDTO()
                {
                    Email = request.UploadStudentDTO.Email,
                    Name = request.UploadStudentDTO.Name
                };

                if (studentRequestDTO == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "Invalid Student data.");
                }

                User user = await _userRepository.FindAsync( d => d.Email == studentRequestDTO.Email);
                UserProfile userProfile = null;
                if (user == null)
                {
                    user = _mapper.Map<User>(request.UploadStudentDTO);
                    string password = _util.GeneratePassword(8);
                    user.Status = true;
                    user.Email = request.UploadStudentDTO.Email;
                    user.loginStatus = true;
                    user.RollNo = request.UploadStudentDTO.RollNo;
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);

                    var role = _roleRepository.Find(r => r.NormalizedName == "STUDENT");

                    UserRole userRole = new UserRole();
                    userRole.UserRoleId = Authorization.Role.STUDENT;
                    userRole.UserId = user.Id;
                    userRole.UserRoleGuid = role.Id;

                    userProfile = _mapper.Map<UserProfile>(user);

                    string result = request.UploadStudentDTO.DegreeName.Replace("\n", "");

                    Department department = await _departmentRepository.FindAsync(d => d.UniversityID.ToString().ToLower() == request.UploadStudentDTO.UniversityId.ToString().ToLower() && d.Type.Replace(" ", "").ToLower() == result.Replace(" ", "").ToLower());

                    DepartmentStudent departmentStudent = new DepartmentStudent()
                    {
                        DepartmentId = department.Id,
                        StudentId = user.Id,
                        UniversityId = request.UploadStudentDTO.UniversityId,
                        StartDate = request.UploadStudentDTO.StartDate,
                        EndDate = request.UploadStudentDTO.EndDate
                    };

                    
                    CreateHandlerResponse createHandlerResponse = await _mediator.Send(new CreateDockHandlerCommand());
                    if (createHandlerResponse == null)
                    {
                        return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Student creation unsuccessfull");
                    }

                    DockIoDID dockDID = new DockIoDID()
                    {
                        DID = createHandlerResponse.Did,
                        UserProfileId = userProfile.Id,
                        Credentials = string.Empty,
                        Password = _configuration.GetSection("Dock:Password").Value
                    };

                    var CreateDockDIDResult = await _mediator.Send(new CertificateDIDCommand(dockDID));

                    string degreeName = request.UploadStudentDTO.DegreeName.Replace("\n", "");
                    string degreeType = request.UploadStudentDTO.DegreeType.Replace("\n", "");

                    Certificate certificateExists = await _certificateRepository.FindAsync(c => c.DegreeName == degreeName && c.DegreeType == degreeType && c.UserProfileId == userProfile.Id);
                    if (certificateExists != null && certificateExists.Status != false )
                    {
                        return new ApiResponse<object>(HttpStatusCode.AlreadyReported, message: "Certificate already exists.");
                    }

                    //IssuerCredentialRequest issuerCredentialRequest = new IssuerCredentialRequest()
                    //{
                    //    userProfileID = userProfile.Id,
                    //    DateOfBirth = request.UploadStudentDTO.DateOfBirth,
                    //    DegreeName = request.UploadStudentDTO.DegreeName,
                    //    DegreeType = request.UploadStudentDTO.DegreeType,
                    //    ExpireDate = request.UploadStudentDTO.ExpireDate,
                    //    IssuanceDate = request.UploadStudentDTO.IssuanceDate,
                    //    StartDate = request.UploadStudentDTO.StartDate,
                    //    EndDate = request.UploadStudentDTO.EndDate,
                    //    Issuer = request.UploadStudentDTO.Issuer,
                    //    Name = request.UploadStudentDTO.Name,
                    //    Password = "0000",
                    //    RecipientEmail = "kundan@bucle.dev",
                    //    Schema = request.UploadStudentDTO.Schema,
                    //    Subject = (request.UploadStudentDTO.Subject),
                    //    Template = request.UploadStudentDTO.CertificateTemplateId.ToString(),
                    //    Email = request.UploadStudentDTO.Email
                    //};

                    //var responseData = await _mediator.Send(new IssuerCredentialCommand(issuerCredentialRequest));

                    //if (!responseData.Success)
                    //{
                    //    return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: responseData.Message);
                    //}
                    //else

                    {
                        User addedUser = await _userRepository.AddAsync(user);
                        if (addedUser == null)
                        {
                            return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Student creation unsuccessfull");
                        }
                        UserProfile addedUserProfile = await _mediator.Send(new UserProfileCommand(userProfile));
                        if(addedUserProfile == null)
                        {
                            return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Student profile creation unsuccessfull");
                        }
                        UserRole addedUserRole =  await _userRoleRepository.AddAsync(userRole);
                        if(addedUserRole == null)
                        {
                            return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Student role creation unsuccessfull");
                        }
                        var addedDepartmentStudent = await _mediator.Send(new CreateDepartmentStudentCommand(departmentStudent));
                        if(addedDepartmentStudent.Success == false)
                        {
                            return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: addedDepartmentStudent.Message );
                        }
                        var subject = "Onboarding on EduBlock";
                        var content = $"<p>Dear {user.Name}, We welcome you on Edublock Platform</p>" +
                        $"<br>" +
                        $"<span> User Id : <strong>{user.Email}</strong></span>" +
                        $"<br>" +
                        $"<span> Password : <strong>{password}</strong></span>";
                        _util.SendSimpleMessage(subject, user.Email, content, "Go to Dashboard", "https://edu-block.org/login");
                        IssuerCredentialRequest issuerCredentialRequest = new IssuerCredentialRequest()
                        {
                            userProfileID = userProfile.Id,
                            DateOfBirth = request.UploadStudentDTO.DateOfBirth,
                            DegreeName = request.UploadStudentDTO.DegreeName,
                            DegreeType = request.UploadStudentDTO.DegreeType,
                            ExpireDate = request.UploadStudentDTO.ExpireDate,
                            IssuanceDate = request.UploadStudentDTO.IssuanceDate,
                            StartDate = request.UploadStudentDTO.StartDate,
                            EndDate = request.UploadStudentDTO.EndDate,
                            Issuer = request.UploadStudentDTO.Issuer,
                            Name = request.UploadStudentDTO.Name,
                            RollNo = request.UploadStudentDTO.RollNo,
                            Password = "0000",
                            RecipientEmail = "kundan@bucle.dev",
                            Schema = request.UploadStudentDTO.Schema,
                            Subject = (request.UploadStudentDTO.Subject),
                            Template = request.UploadStudentDTO.CertificateTemplateId.ToString(),
                            Email = request.UploadStudentDTO.Email
                        };
                        var responseData = await _mediator.Send(new IssuerCredentialCommand(issuerCredentialRequest));
                    }
                }
                else
                {
                    userProfile = await _userProfileRepository.FindAsync(up => up.UserID == user.Id);
                    string degreeName = request.UploadStudentDTO.DegreeName.Replace("\n", "");
                    string degreeType = request.UploadStudentDTO.DegreeType.Replace("\n", "");
                    Certificate certificateExists = await _certificateRepository.FindAsync(c => c.DegreeName == degreeName && c.DegreeType == degreeType && c.UserProfileId == userProfile.Id);
                    if (certificateExists != null && certificateExists.Status != false)
                    {
                        return new ApiResponse<object>(HttpStatusCode.AlreadyReported, message: "Certificate already exists.");
                    }
                    IssuerCredentialRequest issuerCredentialRequest = new IssuerCredentialRequest()
                    {
                        userProfileID = userProfile.Id,
                        DateOfBirth = request.UploadStudentDTO.DateOfBirth,
                        DegreeName = request.UploadStudentDTO.DegreeName,
                        DegreeType = request.UploadStudentDTO.DegreeType,
                        ExpireDate = request.UploadStudentDTO.ExpireDate,
                        IssuanceDate = request.UploadStudentDTO.IssuanceDate,
                        StartDate = request.UploadStudentDTO.StartDate,
                        EndDate = request.UploadStudentDTO.EndDate,
                        Issuer = request.UploadStudentDTO.Issuer,
                        Name = request.UploadStudentDTO.Name,
                        RollNo = request.UploadStudentDTO.RollNo,
                        Password = "0000",
                        RecipientEmail = "kundan@bucle.dev",
                        Schema = request.UploadStudentDTO.Schema,
                        Subject = (request.UploadStudentDTO.Subject),
                        Template = request.UploadStudentDTO.CertificateTemplateId.ToString(),
                        Email = request.UploadStudentDTO.Email
                    };
                    var responseData = await _mediator.Send(new IssuerCredentialCommand(issuerCredentialRequest));
                    if (!responseData.Success)
                    {
                        return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: responseData.Message);
                    }
                }

                if ( userProfile == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: "user does not found");
                }
               
                //var subject = "Onboarding on EduBlock";
                //var content = $"<p>Dear {user.Name}, We welcome you on Edublock Platform</p>" +
                //$"<br>" +
                //$"<span> User Id : <strong>{user.Email}</strong></span>" +
                //$"<br>" +
                //$"<span> Password : <strong>{user.password}</strong></span>";

                //_util.SendSimpleMessage(subject, user.Email, content, null, null);

                //if (addedUser == null)
                //{
                //    return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Student creation unsuccessfull");
                //}

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: user, message: "Student creation successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, error: ex.Message , message: ex.Message);
            }
        }
    }
}