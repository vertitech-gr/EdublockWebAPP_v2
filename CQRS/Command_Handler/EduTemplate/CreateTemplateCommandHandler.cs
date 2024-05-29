using AutoMapper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduRole;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO.EduSchema;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Edu_Block_dev.CQRS.Command_Handler.EduPermission
{
	public class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, ApiResponse<object>>
    {
        private readonly IRepository<Template> _templateRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateTemplateCommandHandler(IRepository<Template> templateRepository, IConfiguration configuration, IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _templateRepository = templateRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                JObject responseObject = await this.CreateTemplateApiCall(request.TemplateDTO);
                var template = new Template()
                {
                    TemplateId = responseObject["id"].ToString(),
                    Name = request.TemplateDTO.Name,
                    UniversityId = request.TemplateDTO.UniversityId,
                    DepartmentId = request.TemplateDTO.DepartmentId,
                    SchemaId = request.TemplateDTO.SchemaId,
                    Html = request.TemplateDTO.HTML,
                    CSS = request.TemplateDTO.CSS,
                };
                await _templateRepository.AddAsync(template);
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: template, message: "added permission");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, error: ex.Message, message: ex.Message);
            }
        }

        public async Task<JObject> CreateTemplateApiCall(TemplateDTO template)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-testnet.dock.io/templates");
            var apiKey = _configuration.GetSection("Dock:ApiKey").Value;
            request.Headers.Add("dock-api-token", apiKey);
            var json = JsonConvert.SerializeObject(template);

            var content = new StringContent(json, null, "application/json");
            request.Content = content;

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                JObject keyValuePairs = JsonConvert.DeserializeObject<JObject>(response);
                return keyValuePairs;
            }
            else
            {
                return new JObject();
            }
        }
    }
}

