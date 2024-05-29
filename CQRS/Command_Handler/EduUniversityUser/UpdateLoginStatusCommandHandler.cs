using System.Net;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUniversity
{
    public class UpdateLoginStatusCommandHandler : IRequestHandler<UpdateLoginStatusCommand, ApiResponse<object>>
    {
        private readonly IRepository<UniversityUser> _universityUserRepository;

        public UpdateLoginStatusCommandHandler(IRepository<UniversityUser> universityUserRepository)
        {
            _universityUserRepository = universityUserRepository;
        }

        public async Task<ApiResponse<object>> Handle(UpdateLoginStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                LoginStatusDTO loginStatusDTO = request.LoginStatusDTO;
                if (loginStatusDTO == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "Invalid university user data.");
                }
                UniversityUser existingUniversityUser = await _universityUserRepository.FindAsync(u => u.Email == loginStatusDTO.Email);
                if (existingUniversityUser == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.Conflict, message: "User don't exits.");
                }
                existingUniversityUser.loginStatus = !existingUniversityUser.loginStatus;
                await _universityUserRepository.UpdateAsync(existingUniversityUser.Id, existingUniversityUser);
                return new ApiResponse<object>( HttpStatusCode.OK, message: "University user login status changed");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>( HttpStatusCode.InternalServerError, error: ex.Message, message: "University creation unsuccessfull");

            }
        }
   }
}
