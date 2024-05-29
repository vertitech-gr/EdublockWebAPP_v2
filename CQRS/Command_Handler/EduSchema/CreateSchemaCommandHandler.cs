using System.Reflection.Emit;
using System.Text;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduRole;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.Dock;
using Edu_Block_dev.Modal.DTO.EduSchema;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Edu_Block_dev.CQRS.Command_Handler.EduPermission
{
	public class CreateSchemaCommandHandler : IRequestHandler<CreateSchemaCommand, ApiResponse<object>>
    {
        private readonly IRepository<DAL.EF.Schema> _schemaRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateSchemaCommandHandler(IRepository<DAL.EF.Schema> schemaRepository, IRepository<Template> templateRepository, IConfiguration configuration, IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _schemaRepository = schemaRepository;
            _templateRepository = templateRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(CreateSchemaCommand request, CancellationToken cancellationToken)
        {
            try
            {

                JObject output = await this.CreateSchemaApiCall(request.SchemaDTO);
                if (output.ContainsKey("data"))
                {
                    var schema = new Schema
                    {
                        Id = Guid.NewGuid(),
                        Name = request.SchemaDTO.Name,
                        UniversityId = request.SchemaDTO.UniversityId,
                        Description = request.SchemaDTO.Description,
                        SchemaId = output["data"]["id"].ToString(),
                        Attributes = request.SchemaDTO.Attributes
                    };
                    await _schemaRepository.AddAsync(schema);

                    return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: schema, message: "added schema");
                }

                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, data: null, message: "added schema");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, error: ex.Message, message: "added schema");
            }
        }

        public async Task<JObject> CreateSchemaApiCall(SchemaDTO SchemaDTO)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-testnet.dock.io/schemas");
            var apiKey = _configuration.GetSection("Dock:ApiKey").Value;
            request.Headers.Add("dock-api-token", apiKey);

            //var attribute = JsonConvert.DeserializeObject<JObject>(SchemaDTO.Attributes);
            List<Propeties> properties = new List<Propeties>(); 

            var jsonContent = @"{
            ""$schema"": ""http://json-schema.org/draft-07/schema#"",
            ""$id"": ""https://schema.dock.io/type.json"",
            ""type"": ""object"",
            ""$metadata"": {
                ""version"": 1,
                ""uris"": {}
            },
            ""required"": [
                ""@context"",
                ""id"",
                ""type"",
                ""issuanceDate"",
                ""credentialSubject"",
                ""credentialSchema"",
                ""issuer""
            ],
            ""properties"": {
                ""@context"": {
                    ""type"": [
                        ""string"",
                        ""array"",
                        ""object""
                    ]
                },
                ""id"": {
                    ""type"": ""string""
                },
                ""type"": {
                    ""type"": [
                        ""string"",
                        ""array""
                    ],
                    ""items"": {
                        ""type"": ""string""
                    }
                },
                ""issuer"": {
                    ""type"": [
                        ""string"",
                        ""object""
                    ],
                    ""format"": ""uri"",
                    ""required"": [
                        ""id""
                    ],
                    ""properties"": {
                        ""id"": {
                            ""type"": ""string"",
                            ""format"": ""uri""
                        }
                    }
                },
                ""issuanceDate"": {
                    ""type"": ""string"",
                    ""format"": ""date-time""
                },
                ""expirationDate"": {
                    ""type"": ""string"",
                    ""format"": ""date-time""
                },
                ""credentialStatus"": {
                    ""type"": ""object"",
                    ""required"": [
                        ""id"",
                        ""type""
                    ],
                    ""properties"": {
                        ""id"": {
                            ""type"": ""string"",
                            ""format"": ""uri""
                        },
                        ""type"": {
                            ""type"": ""string""
                        }
                    }
                },
                ""credentialSchema"": {
                    ""type"": ""object"",
                    ""required"": [
                        ""id"",
                        ""type""
                    ],
                    ""properties"": {
                        ""id"": {
                            ""type"": ""string"",
                            ""format"": ""uri""
                        },
                        ""type"": {
                            ""type"": ""string""
                        }
                    }
                },
                ""subjectPosition"": {
                    ""type"": ""string"",
                    ""enum"": [
                        ""none"",
                        ""index"",
                        ""value""
                    ]
                },
                ""merklizationRootPosition"": {
                    ""type"": ""string"",
                    ""enum"": [
                        ""none"",
                        ""index"",
                        ""value""
                    ]
                },
                ""revNonce"": {
                    ""type"": ""integer""
                },
                ""version"": {
                    ""type"": ""integer""
                },
                ""updatable"": {
                    ""type"": ""boolean""
                },
                ""name"": {
                    ""type"": ""string""
                },
                ""description"": {
                    ""type"": ""string""
                },
                ""credentialSubject"": {
                    ""type"": ""object"",
                    ""properties"": {
                        ""id"": {
                            ""title"": ""ID"",
                            ""type"": ""string"",
                            ""description"": ""A unique identifier of the recipient. Example: DID, email address, national ID number, employee ID, student ID etc. If you enter the recipient's DID, the person will automatically receive the credential in their Dock wallet.""
                        }
                    },
                    ""required"": [
                       
                    ]
                }
            },
   ""name"": """ + SchemaDTO.Name + @""",
            ""description"": """ + SchemaDTO.Description + @"""

}"
            ;

          
            //     ""name"": """ + SchemaDTO.Name + @""",
            //""description"": """ + SchemaDTO.Description + @""",

            var schema = JsonConvert.DeserializeObject<JObject>(jsonContent);
            var propertiesSchema = (JObject)schema["properties"];
            var credentialSubjectObject = (JObject)propertiesSchema["credentialSubject"];
            var propertiesObject = (JObject)credentialSubjectObject["properties"];

            var array = JsonConvert.DeserializeObject<JArray>(SchemaDTO.Attributes);

            foreach (var item in array)
            {
                if (item is JObject) 
                {
                    var attributeObject = (JObject)item;
                    var property = new Propeties();
                    property.title = (string)attributeObject["name"];
                    property.type = (string)attributeObject["type"];
                    property.description = (string)attributeObject["description"];

                    propertiesObject.Add(property.title, JObject.FromObject(property));
                    properties.Add(property);
                }
                else if (item is JValue) 
                {
                    var value = (string)item;
                }
            }

            //credentialSubjectObject["properties"] = propertiesObject;

            request.Content = new StringContent(schema.ToString(), Encoding.UTF8, "application/json");
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

