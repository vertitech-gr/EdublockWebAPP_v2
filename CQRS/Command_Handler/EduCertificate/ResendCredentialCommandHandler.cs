using Edu_Block.DAL;
using MediatR;
using AutoMapper;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class ResendCredentialCommandHandler : IRequestHandler<ResendCredentialCommand, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly Util _util;


        public ResendCredentialCommandHandler(Util util,IConfiguration configuration,IMapper mapper, EduBlockDataContext context, IRepository<Certificate> certificateRepository)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _util = util;
        } 

        public async Task<ApiResponse<object>> Handle(ResendCredentialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                List<UserProfileCertificateDTO> userCertificates =  _context.Certificates.Where(c => c.Id == request.Guid)
                    .Join(_context.UserProfiles, c => c.UserProfileId, up => up.Id, (c, up) => new { Certificate = c, UserProfile = up })
                    .Join(_context.User, cup => cup.UserProfile.UserID, u => u.Id, (cup, u) => new UserProfileCertificateDTO
                    {
                        Certificate = cup.Certificate ,
                        UserProfile = cup.UserProfile,
                        User = u
                    }).ToList();
                foreach (UserProfileCertificateDTO userCertificate in userCertificates)
                {
                    var subject = "Certificate link on EduBlock";
                    var content = $"<p>Dear {userCertificate.UserProfile.Name}, We welcome you on Edublock Platform</p>" +
                    $"<br>" +
                    $"<span> User Id : <strong>{userCertificate.User.Email}</strong></span>" +
                    $"<br>" +
                    $"<span> Link : <strong>{_configuration.GetSection("Base:url").Value + "/" + "Certificate" + "/" + userCertificate.Certificate.FileName}</strong></span>";
                    _util.SendSimpleMessage(subject, userCertificate.User.Email, content, "View Certificate", _configuration.GetSection("Base:url").Value + "/" + "Certificate" + "/" + userCertificate.Certificate.FileName);
                }
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: userCertificates,  message: "Certificate update successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError,  message: "Certificate update Unsuccessfull");

            }
        }
    }
}