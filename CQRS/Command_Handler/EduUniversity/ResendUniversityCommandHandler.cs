using System.Net;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUniversity
{
    public class ResendUniversityCommandHandler : IRequestHandler<ResendUniversityCommand, ApiResponse<object>>
    {
        private readonly IRepository<University> _universityRepository;
        private readonly Util _util;
        private readonly IConfiguration _configuration;

        public ResendUniversityCommandHandler(IRepository<University> universityRepository, Util util, IConfiguration configuration)
        {
            _universityRepository = universityRepository;
            _util = util;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(ResendUniversityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                University existingUniversity = await _universityRepository.FindAsync(u => u.Id == request.Guid);
                if (existingUniversity ==  null)
                {
                    return new ApiResponse<object>(HttpStatusCode.NotFound, message: "Unable to find university");

                }
                var newPassword = _util.GeneratePassword(8);
                existingUniversity.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                var addedUniversity = _universityRepository.UpdateAsync(existingUniversity.Id,existingUniversity);
                if (addedUniversity == null)
                {
                    return new ApiResponse<object>( HttpStatusCode.InternalServerError, message: "University update unsuccessfull");
                }

                string link = _configuration.GetSection("DashboardUrl:University").Value + "/login";

                var subject = "Onboarding on EduBlock";
                var content = $"<p>Dear {existingUniversity.Name}, We welcome you on Edublock Platform</p>" +
                $"<br>" +
                $"<span> User Id : <strong>{existingUniversity.Email}</strong></span>" +
                $"<br>" +
                $"<span> Password : <strong>{newPassword}</strong></span>" +
                $"<br>" +
                $"<span>Click on this link : <strong>{link}</strong></span>";

                _util.SendSimpleMessage(subject, existingUniversity.Email, content, "Go to dashboard", link);

                return new ApiResponse<object>( HttpStatusCode.OK, message: "Check email Click on the given link");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>( HttpStatusCode.InternalServerError, message: "University update unsuccessfull");

            }
        }
   }
}
