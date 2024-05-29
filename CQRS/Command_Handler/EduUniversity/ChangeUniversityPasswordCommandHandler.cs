using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using System.Net;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class ChangeUniversityPasswordCommandHandler : IRequestHandler<ChangeUniversityPasswordCommand, ApiResponse<object>>
    {
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<UniversityUser> _universityUserRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Role> _roleRepository;

        public ChangeUniversityPasswordCommandHandler(IRepository<UserProfile> userProfileRepository, IRepository<UniversityUser> universityUserRepository, IRepository<University> universityRepository, IRepository<Role> roleRepository)
        {
            _universityRepository = universityRepository;
            _userProfileRepository = userProfileRepository;
            _universityUserRepository = universityUserRepository;
            _roleRepository = roleRepository;
        }

        public async Task<ApiResponse<object>> Handle(ChangeUniversityPasswordCommand command, CancellationToken cancellationToken)
        {
            try
            {
                Role role = await _roleRepository.FindAsync(u => u.Id == command.RoleId);
                Role roleInstitution = await _roleRepository.FindAsync(r => r.NormalizedName == "INSTITUTION");

                if (role == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.NotFound, data: null, message: "Unable to login");
                }

                var userProfile = await _userProfileRepository.FindAsync(u => u.Id == command.CommanUser.UserProfile.Id);

                if (role.Id == roleInstitution.Id)
                {
                    var existingUniversity = await _universityRepository.FindAsync(u => u.Id == userProfile.UserID);
                    existingUniversity.Password = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
                    existingUniversity.Status = true;
                    if (!existingUniversity.Active)
                    {
                        existingUniversity.Active = true;
                    }
                    _universityRepository.Update(existingUniversity.Id, existingUniversity);

                }
                else
                {
                    var existingUniversityUser = await _universityUserRepository.FindAsync(u => u.Id == userProfile.UserID);
                    existingUniversityUser.Password = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
                    existingUniversityUser.Status = true;
                    if (!existingUniversityUser.Active)
                    {
                        existingUniversityUser.Active = true;
                    }
                    _universityUserRepository.Update(existingUniversityUser.Id, existingUniversityUser);

                }

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK,message: "Password changed successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError,message: "Password change Unsuccessfull");

            }
        }
    }
}