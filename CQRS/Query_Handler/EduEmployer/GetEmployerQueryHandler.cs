using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduEmployer;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduEmployer
{
    public class GetEmployerQueryHandler : IRequestHandler<GetEmployerQuery, EmployerDTO>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<UserProfile> _userProfile;
        public GetEmployerQueryHandler(IRepository<UserProfile> userProfile, EduBlockDataContext context)
        {
            _context = context;
            _userProfile = userProfile;
        }

        public async Task<EmployerDTO> Handle(GetEmployerQuery request, CancellationToken cancellationToken)
        {
            UserProfile employerProfile = await _context.UserProfiles.FindAsync( request.EmployerId );
            var employer = await _context.Employers.FindAsync(employerProfile.UserID);


            if (employer == null)
                return null;

            var employerDTO = new EmployerDTO
            {
                Name = employer.Name,
                Email = employer.Email,
                Address = employer.Address,
                Industry = employer.Industry,
                SpecificIndustry = employer.SpecificIndustry,
                //EmployerProfileId = employerProfile.Id
            };

            return employerDTO;
        }
    }
}
