using AutoMapper;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using EduBlock.Model.DTO;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetEmployeeByProfileIdQueryHandler : IRequestHandler<GetEmployeeByProfileIdQuery, UserWithProfile<Employer>>
    {
        private readonly IRepository<Employer> _employerRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<DockIoDID> _dockRepository;
        private readonly IMapper _mapper;

        public GetEmployeeByProfileIdQueryHandler(IRepository<UserProfile> userProfileRepository, IRepository<Employer> employerRepository, IRepository<DockIoDID> dockRepository, IMapper mapper)
        {
            _employerRepository = employerRepository;
            _userProfileRepository = userProfileRepository;
            _dockRepository = dockRepository;
            _mapper = mapper;
        }

        public async Task<UserWithProfile<Employer>> Handle(GetEmployeeByProfileIdQuery request, CancellationToken cancellationToken)
        {
            UserProfile profile = await _userProfileRepository.FindAsync(u => u.Id == request._uniqueId);
            Employer employer = await _employerRepository.FindAsync(u => u.Id == profile.UserID);
            DockIoDID dock = await _dockRepository.FindAsync(u => u.UserProfileId == profile.UserID);
            return new UserWithProfile<Employer>(employer, profile, dock);
        }
    }
}
