using AutoMapper;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using EduBlock.Model.DTO;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetEmployeeByUniqueIdQueryHandler : IRequestHandler<GetEmployeeByUniqueIdQuery, UserWithProfile<Employer>>
    {
        private readonly IRepository<Employer> _employerRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<DockIoDID> _dockRepository;
        private readonly IMapper _mapper;

        public GetEmployeeByUniqueIdQueryHandler(IRepository<UserProfile> userProfileRepository, IRepository<Employer> employerRepository, IRepository<DockIoDID> dockRepository, IMapper mapper)
        {
            _employerRepository = employerRepository;
            _userProfileRepository = userProfileRepository;
            _dockRepository = dockRepository;
            _mapper = mapper;
        }

        public async Task<UserWithProfile<Employer>> Handle(GetEmployeeByUniqueIdQuery request, CancellationToken cancellationToken)
        {
            //UserProfile profile = await _userProfileRepository.FindAsync(u => u.Id == request._uniqueId);
            //Employer employer = await _employerRepository.FindAsync(u => u.Id == profile.UserID);

            Employer employer = await _employerRepository.FindAsync(u => u.Id == request._uniqueId);
            UserProfile profile = await _userProfileRepository.FindAsync(u => u.UserID == request._uniqueId);
            DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == profile.Id);

            return new UserWithProfile<Employer>(employer, profile, dock);
        }
    }
}
