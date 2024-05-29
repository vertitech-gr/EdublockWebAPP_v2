using System.Globalization;
using CsvHelper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduDepartment;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;
using Newtonsoft.Json;

namespace Edu_Block_dev.CQRS.Command_Handler.EduDepartment
{
    public class UploadStudentCommandHandler : IRequestHandler<UploadStudentCommand, ApiResponse<object>>
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IRepository<Schema> _schemaRepository;
        private readonly IConfiguration _configuration;

        public UploadStudentCommandHandler(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IRepository<Schema> schemaRepository, IMediator mediator)
        {
            _mediator = mediator;
            _configuration = configuration;
            _schemaRepository = schemaRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ApiResponse<object>> Handle(UploadStudentCommand request, CancellationToken cancellationToken)
        {
            try {
                if (request.UploadStudentRequestDTO.File != null && request.UploadStudentRequestDTO.File.Length > 0)
                {
                    List<UploadStudentDTO> unableToaddUsers = new List<UploadStudentDTO>();
                    List<UploadStudentDTO> UploadStudentDTOs = new List<UploadStudentDTO>();

                    string dynamicFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "CSV");
                    try
                    {
                        if (!Directory.Exists(dynamicFolderPath))
                        {
                            Directory.CreateDirectory(dynamicFolderPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: "Unable to create Envelope.");
                    }

                    Schema schema = await _schemaRepository.FindAsync( s => s.SchemaId == request.UploadStudentRequestDTO.Schema);
                    string mappedArray = request.UploadStudentRequestDTO.MappedArray;
                    List<SchemaAttribute> schemaData = JsonConvert.DeserializeObject<List<SchemaAttribute>>(schema.Attributes);
                    List<MappedArrayDTO> mappedArrayData = JsonConvert.DeserializeObject<List<MappedArrayDTO>>(mappedArray);

                    using (var reader = new StreamReader(request.UploadStudentRequestDTO.File.OpenReadStream()))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var studentRecords = GetRecordsDynamic(csv, request.UploadStudentRequestDTO.MappedArray);
                        foreach(var studentRecord in studentRecords)
                        {

                            var SchemaObject = new Dictionary<string, object>();
                            var CredentialObject = new Dictionary<string, object>();

                            UploadStudentDTO uploadStudentDTO = new UploadStudentDTO();
                            uploadStudentDTO.Email = studentRecord["Email"].ToString();
                            uploadStudentDTO.DegreeName = request.UploadStudentRequestDTO.DegreeName;
                            uploadStudentDTO.CertificateTemplateId = request.UploadStudentRequestDTO.CertificateTemplateId;
                            uploadStudentDTO.UniversityId = request.UploadStudentRequestDTO.UniversityId;
                            uploadStudentDTO.Issuer = request.UploadStudentRequestDTO.Issuer;
                            uploadStudentDTO.Schema = request.UploadStudentRequestDTO.Schema;

                            var subjectItem = mappedArrayData.FirstOrDefault(item => item.Name == "subject");
                            if (subjectItem != null)
                            {
                                foreach (var attribute in subjectItem.Attributes)
                                {
                                    var matchingAttribute = schemaData.Find(sd => sd.Name.ToLower() == attribute.Name.ToLower() );
                                    if (matchingAttribute != null)
                                    {
                                        SchemaObject.Add(matchingAttribute.Name, studentRecord[attribute.Value]);
                                    }
                                    if (attribute.Name.ToLower() == "id")
                                    {
                                        SchemaObject.Add(attribute.Name, studentRecord[attribute.Value]);
                                    }
                                }
                            }

                            var credentialItem = mappedArrayData.FirstOrDefault(item => item.Name == "credential");
                            if (credentialItem != null)
                            {
                                foreach (var attribute in credentialItem.Attributes)
                                {
                                    if(attribute.Name == "issueDate")
                                    {
                                        uploadStudentDTO.IssuanceDate = DateTime.ParseExact(studentRecord[attribute.Value].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    }

                                    if (attribute.Name == "expireDate")
                                    {
                                        //uploadStudentDTO.EndDate = DateTime.ParseExact(studentRecord[attribute.Value].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    }
                                }
                            }

                            var commonItem = mappedArrayData.FirstOrDefault(item => item.Name.ToLower() == "common");
                            if (commonItem != null)
                            {
                                foreach (var attribute in commonItem.Attributes)
                                {
                                    if (attribute.Name.ToLower() == "name")
                                    {
                                        uploadStudentDTO.Name = studentRecord[attribute.Value].ToString();
                                    }

                                    if (attribute.Name.ToLower() == "email")
                                    {
                                        uploadStudentDTO.Email = studentRecord[attribute.Value].ToString();
                                    }

                                    if (attribute.Name.ToLower() == "rollno")
                                    {
                                        uploadStudentDTO.RollNo = studentRecord[attribute.Value].ToString();
                                    }

                                    if (attribute.Name.ToLower() == "degreetype")
                                    {
                                        uploadStudentDTO.DegreeType = studentRecord[attribute.Value].ToString();

                                    }
                                    if (attribute.Name.ToLower() == "degreeawardeddate")
                                    {
                                        uploadStudentDTO.DegreeAwardedDate = DateTime.ParseExact(studentRecord[attribute.Value].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    }
                                    if (attribute.Name.ToLower() == "dateofbirth")
                                    {
                                        uploadStudentDTO.DateOfBirth = DateTime.ParseExact(studentRecord[attribute.Value].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    }
                                    if (attribute.Name.ToLower() == "startdate")
                                    {
                                        uploadStudentDTO.StartDate = DateTime.ParseExact(studentRecord[attribute.Value].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    }
                                    if (attribute.Name.ToLower() == "enddate")
                                    {
                                        uploadStudentDTO.EndDate = DateTime.ParseExact(studentRecord[attribute.Value].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    }

                                }
                            }

                            uploadStudentDTO.Subject = JsonConvert.SerializeObject(SchemaObject);

                            if (SchemaObject.Count == 0)
                            {
                                schemaData.ForEach(sd => {
                                    SchemaObject.Add(sd.Name, "");
                                });
                            }

                            uploadStudentDTO.Subject = JsonConvert.SerializeObject(SchemaObject);

                            var result = await _mediator.Send(new CreateStudentCommand(uploadStudentDTO));
                            if (result.Success)
                            {
                            }
                            else
                            {
                                uploadStudentDTO.Remarks = result.Message;
                                unableToaddUsers.Add(uploadStudentDTO);
                            }
                        }
                    }

                    string dynamicFilePath = string.Empty;
                    string csvFileName = string.Empty;
                    if (unableToaddUsers != null && unableToaddUsers.Count() > 0 )
                    {
                        csvFileName = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".csv";
                        dynamicFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "CSV",csvFileName);
                        WriteCsvFile(unableToaddUsers, dynamicFilePath);
                    }
                    return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { csvUrl = dynamicFilePath != string.Empty ? _configuration.GetSection("Base:Url").Value + "/CSV/" + csvFileName  : string.Empty  , unableToaddUsers }, message: "Department updated successfully");
                }
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: null, message: "Department updated successfully");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError , message: ex.Message);
            }
        }

        public void WriteCsvFile(List<UploadStudentDTO> students, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(students);
            }
        }

        public static IEnumerable<Dictionary<string, object>> GetRecordsDynamic(CsvReader csv, string mappedArray)
        {
            var records = new List<Dictionary<string, object>>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new Dictionary<string, object>();
                foreach (var header in csv.HeaderRecord)
                {
                    record[header] = csv.GetField(header);
                }
                records.Add(record);
            }
            return records;
        }
    }

}

    //public class MyCsvClassMap: ClassMap<UploadStudentDTO>
    //{
    //    public MyCsvClassMap()
    //    {
    //        Map(m => m.Email);
    //        Map(m => m.Name);
    //        Map(m => m.DegreeType);
    //        Map(m => m.DegreeAwardedDate).TypeConverterOption.Format("dd/MM/yyyy");
    //        Map(m => m.DateOfBirth).TypeConverterOption.Format("dd/MM/yyyy");
    //        Map(m => m.IssuanceDate).TypeConverterOption.Format("dd/MM/yyyy");
    //        Map(m => m.ExpireDate).TypeConverterOption.Format("dd/MM/yyyy");
    //        Map(m => m.StartDate).TypeConverterOption.Format("dd/MM/yyyy");
    //        Map(m => m.EndDate).TypeConverterOption.Format("dd/MM/yyyy");
    //        Map(m => m.Subject);
    //    }
    //}
