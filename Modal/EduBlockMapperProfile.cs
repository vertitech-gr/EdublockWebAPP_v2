using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.Dock;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;

public class Applicationmapper : Profile
{
    public Applicationmapper()
    { 
        CreateMap<UserDTO, User>();
        CreateMap<UploadStudentDTO, User>();
        CreateMap<StudentRequestDTO, User>();
        CreateMap<User, UserDetailsDTO>();
        CreateMap<UserRequest, UserRequestDto>().ReverseMap();

        CreateMap<WebhookPayloadDTO, PaymentDetail>().ReverseMap();

        CreateMap<CommanUser, User>()
          .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
          .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)).ReverseMap();

        CreateMap<CommanUser, University>()
       .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
       .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
       .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)).ReverseMap();



        CreateMap<DepartmentDTO, Department>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.UniversityID, opt => opt.MapFrom(src => src.UniversityID));

        CreateMap<UserDTO, Admin>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<RequestMessageDTO, RequestMessage>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.RequestId))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

        CreateMap<PermissionRequestDTO, Permission>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<UserProfile, Otp>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.UserProfileId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.OtpType, opt => opt.MapFrom(src => OtpType.REGISTER));

        CreateMap<Admin, UserProfile>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id ))
            .ForMember<string>(dest => dest.Description, opt => opt.MapFrom(src => src.Name))
            .ForMember<string>(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<User, UserProfile>()
          .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
          .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id))
          .ForMember<string>(dest => dest.Description, opt => opt.MapFrom(src => src.Name))
          .ForMember<string>(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<UniversityUser, UserProfile>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id))
            .ForMember<string>(dest => dest.Description, opt => opt.MapFrom(src => src.Name))
            .ForMember<string>(dest => dest.Name, opt => opt.MapFrom(src => src.Name));


        CreateMap<University, UserProfile>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
           .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id))
           .ForMember<string>(dest => dest.Description, opt => opt.MapFrom(src => src.Name))
           .ForMember<string>(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<Department, UserProfile>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
           .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id))
           .ForMember<string>(dest => dest.Description, opt => opt.MapFrom(src => src.Name))
           .ForMember<string>(dest => dest.Name, opt => opt.MapFrom(src => src.Name));


        CreateMap<University, UserRole>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(src => Edu_Block_dev.Authorization.Role.UNIVERSITY))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));


        CreateMap<UniversityUser, UserRole>()
          .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
          .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(src => Edu_Block_dev.Authorization.Role.UNIVERSITYUSER))
          .ForMember(dest => dest.UserRoleGuid, opt => opt.MapFrom(src => src.RoleId))
          .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));


        CreateMap<Department, UserRole>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(src => Edu_Block_dev.Authorization.Role.DEPARTMENT))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));



        CreateMap<Admin, UserRole>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(src => Edu_Block_dev.Authorization.Role.ADMIN))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

        CreateMap<ShareDTO, Share>()
            .ForMember(dest => dest.ReceiverId, opt => opt.MapFrom(src => src.EmailId))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.RequsetId, opt => opt.MapFrom(src => src.RequestId == Guid.Empty ? Guid.NewGuid() : src.RequestId))
            .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.ResourceId));

        CreateMap<UniversityRequestDTO, University>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<UniversityUserRequestDTO, UniversityUser>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
           //.ForMember(dest => dest.UniversityId, opt => opt.MapFrom(src => src.UniversityId))
           .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
           //.ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        CreateMap<UniversityUserRequestDTO, UniversityDepartmentUser>()
           .ForMember(dest => dest.UniversityId, opt => opt.MapFrom(src => src.UniversityId))
           .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId));


        CreateMap<UniversityUpdateRequestDTO, University>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<University, UniversityResponseDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<UniversityRequestDTO, UniversityDetail>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<University, UserProfile>()
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Name));

        //CreateMap<DepartmentDTO, IssuerDTO>()
        //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
        //    .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Logo));

        CreateMap<UniversityRequestDTO, IssuerDTO>()
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
           .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Logo));


        CreateMap<EmployerDTO, Employer>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.Industry))
            .ForMember(dest => dest.SpecificIndustry, opt => opt.MapFrom(src => src.SpecificIndustry));

        CreateMap<Employer, EmployerDTO>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.Industry))
            .ForMember(dest => dest.SpecificIndustry, opt => opt.MapFrom(src => src.SpecificIndustry));
      
        CreateMap<Employer, EmployerResponseDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.Industry))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.SpecificIndustry, opt => opt.MapFrom(src => src.SpecificIndustry));

        CreateMap<RoleRequestDTO, Role>()
            .ForMember(dest => dest.NormalizedName, opts => opts.MapFrom(src => src.Name.ToUpper()))
            .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name));

        CreateMap<UpdateRoleRequestDTO, Role>()
             .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Guid))
            .ForMember(dest => dest.ConcurrencyStamp, opts => opts.MapFrom(src => src.ConcurrencyStamp))
            .ForMember(dest => dest.NormalizedName, opts => opts.MapFrom(src => src.Name.ToUpper()))
            .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name));

        CreateMap<EmployeeRequestDTO, Request>()
            .ForMember(dest => dest.ReceiverId, opts => opts.MapFrom(src => src.ReceiverId));

        CreateMap<Request, EmployeeRequestDTO>()
            .ForMember(dest => dest.ReceiverId, opts => opts.MapFrom(src => src.ReceiverId));

        CreateMap<EmployeeRequestGroupDTO, EmployeeRequestGroup>()
            .ForMember(dest => dest.RequestId, opts => opts.MapFrom(src => src.RequestId))
            .ForMember(dest => dest.CertificateId, opts => opts.MapFrom(src => src.CertificateId))
            .ForMember(dest => dest.Status, opts => opts.MapFrom(src => Enum.Parse<RequestStatus>(src.Status.ToString())));

        CreateMap<EmployeeRequestGroup, EmployeeRequestGroupDTO>()
            .ForMember(dest => dest.RequestId, opts => opts.MapFrom(src => src.RequestId))
            .ForMember(dest => dest.CertificateId, opts => opts.MapFrom(src => src.CertificateId))
            .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status.ToString()));

        CreateMap<EnvelopGroup, EnvelopeRequestGroupDTO>()
            .ForMember(dest => dest.EnvelopeId, opts => opts.MapFrom(src => src.EnvelopeID))
            .ForMember(dest => dest.CertificateId, opts => opts.MapFrom(src => src.CertificateId))
            .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status.ToString()));

        CreateMap<Share, ShareRespnseDto>()
            .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
            .ForMember(dest => dest.ReceiverId, opts => opts.MapFrom(src => src.ReceiverId))
            .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Type.ToString()));

        CreateMap<ShareCredential, ShareCredentialDTO>()
            .ForMember(dest => dest.ShareId, opts => opts.MapFrom(src => src.ShareId))
            .ForMember(dest => dest.CredentialId, opts => opts.MapFrom(src => src.CredentialId));

    }
}
