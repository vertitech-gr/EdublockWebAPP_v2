using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using AutoMapper;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUser
{
    public class UniversityDetailCommandHandler : IRequestHandler<UniversityDetailsCommand, ApiResponse<object>>
    {
        private readonly IRepository<UniversityDetail> _universityDetailRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UniversityDetailCommandHandler(IRepository<UniversityDetail> universityDetailRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            _universityDetailRepository = universityDetailRepository;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ApiResponse<object>> Handle(UniversityDetailsCommand request, CancellationToken cancellationToken)
        {

            try {
                UniversityDetail universityDetail = _mapper.Map<UniversityDetail>(request.UniversityRequestDTO);
                if (request.UniversityRequestDTO.Logo != null)
                {
                    var fileName = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + Path.GetExtension(request.UniversityRequestDTO.Logo.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "University", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.UniversityRequestDTO.Logo.CopyToAsync(fileStream);
                    }
                    universityDetail.Logo = Path.Combine("University", fileName);
                }

                universityDetail.UniversityId = request.UniversityId;
                await _universityDetailRepository.AddAsync(universityDetail);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, message: "University Detail Added successfully.");

            }
            catch (Exception ee)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: "Unable to add university details.");

            }
        }
    }
}