using AutoMapper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduRole;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduDepartment
{
    public class RequestMessageCommandHandler : IRequestHandler<RequestMessageCommand, ApiResponse<object>>
    {
        private readonly IRepository<RequestMessage> _requestMessageRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public RequestMessageCommandHandler(IRepository<RequestMessage> requestMessageRepository, IWebHostEnvironment hostingEnvironment, IMapper mapper)
        {
            _requestMessageRepository = requestMessageRepository;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ApiResponse<object>> Handle(RequestMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                RequestMessage requestMessage = _mapper.Map<RequestMessage>(request.RequestMessageDTO);
             
                var imageFile = request.RequestMessageDTO.Attachment;
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var uploadDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "IMG");
                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }
                    var filePath = Path.Combine(uploadDirectory, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    requestMessage.Attachment = "IMG/" + uniqueFileName;
                }

                var addedDepartment = await _requestMessageRepository.AddAsync(requestMessage);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: requestMessage, message: "Department creation successfully");

            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, data: null , message: "Department creation unsuccessfull");
            }
        }
    }
}