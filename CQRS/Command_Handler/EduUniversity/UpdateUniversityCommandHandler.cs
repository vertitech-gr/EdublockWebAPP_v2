using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block.DAL.EF;
using AutoMapper;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class UpdateUniversityCommandHandler : IRequestHandler<UpdateUniversityCommand, ApiResponse<object>>
    {
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<UniversityDetail> _universityDetailRepository;
        private readonly IMapper _mapper;

        public UpdateUniversityCommandHandler(IRepository<UniversityDetail> universityDetailRepository, IMapper mapper, IRepository<University> universityRepository)
        {
            _universityRepository = universityRepository;
            _universityDetailRepository = universityDetailRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<object>> Handle(UpdateUniversityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                University existingUniversity = await _universityRepository.FindAsync(u => u.Id == request.Guid );
                UniversityDetail universityDetail = await _universityDetailRepository.FindAsync(u => u.UniversityId == existingUniversity.Id );
                existingUniversity.Name = request.UniversityUpdateRequestDTO.Name;
                existingUniversity.Type = request.UniversityUpdateRequestDTO.Type;
                existingUniversity.PhoneNumber = request.UniversityUpdateRequestDTO.PhoneNumber;
                existingUniversity.Status = request.UniversityUpdateRequestDTO.Status;
                existingUniversity.UpdatedAt = DateTime.UtcNow;
                existingUniversity.loginStatus = request.UniversityUpdateRequestDTO.LoginStatus;
                existingUniversity.Active = request.UniversityUpdateRequestDTO.Active;
                existingUniversity.UpdatedBy = request.Admin.UserProfile.Id;
                await _universityRepository.UpdateAsync(existingUniversity.Id, existingUniversity);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK,message: "University update successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError,message: "University update Unsuccessfull");

            }
        }
    }
}