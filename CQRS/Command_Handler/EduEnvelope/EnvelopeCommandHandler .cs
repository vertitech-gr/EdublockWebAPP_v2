using DinkToPdf;
using DinkToPdf.Contracts;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduEnvelope;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Edu_Block_dev.Modal.Enum;

namespace Edu_Block_dev.CQRS.Command_Handler.EduEnvelope
{
    public class EnvelopeCommandHandler : IRequestHandler<EnvelopeCommand, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;
        private readonly IRepository<EnvelopGroup> _envelopeGroup;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConverter _converter;
        private readonly Util _util;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Envelope> _envelope;

        public EnvelopeCommandHandler(EduBlockDataContext context, IRepository<Envelope> envelope, IConfiguration configuration, IMediator mediator, Util util, IConverter converter, IRepository<EnvelopGroup> envelopeGroup, IWebHostEnvironment hostingEnvironment)
        {
            _mediator = mediator;
            _context = context;
            _envelopeGroup = envelopeGroup;
            _hostingEnvironment = hostingEnvironment;
            _converter = converter;
            _util = util;
            _configuration = configuration;
            _envelope = envelope;
        }

        public async Task<ApiResponse<object>> Handle(EnvelopeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.envelopeDTOs == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "Unable to create Envelope.");
                }

                var existingEnvelopeName = request.envelopeDTOs.Name;
                var webRootPath = _configuration.GetSection("Base:CertificatePath").Value;
                string dynamicFolderName = request?.envelopeDTOs.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                dynamicFolderName = "envelopes_" + request.User.UserProfile.Id + Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(dynamicFolderName))).Replace("/","").Replace("+", "");
                string dynamicFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, dynamicFolderName);
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
                var _envelopes = await _envelope.FindAsync((e) => e.Name == request.envelopeDTOs.Name && e.UserProfileId == request.User.UserProfile.Id);
                if (_envelopes != null && EnvelopeShareType.SHARE == request.Type)
                {
                    request.envelopeDTOs.Name = request.envelopeDTOs.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                if(_envelopes != null && EnvelopeShareType.CREATE == request.Type)
                {
                    return new ApiResponse<object>( System.Net.HttpStatusCode.Conflict, data: null,message: "Envelope name alredy exists.");
                }
                var envelopEntity = MapDepartmentDtoToEntity(request.envelopeDTOs, dynamicFolderName, request.User, EnvelopeShareType.SHARE == request.Type ? EnvelopeShareType.SHARE : EnvelopeShareType.CREATE );
                _context.Add(envelopEntity);
                await _context.SaveChangesAsync();
                await _mediator.Send(new EnvelopQueryById(envelopEntity.Id));
                try
                {
                    foreach (var credential in request.envelopeDTOs.Credentials)
                    {
                        EnvelopeRequestGroupDTO envelopeRequestGroupDTO = new EnvelopeRequestGroupDTO()
                        {
                            CertificateId = credential,
                            EnvelopeId = envelopEntity.Id,
                        };
                        await _mediator.Send(new EnvelopeGroupCommand(envelopeRequestGroupDTO));
                        string pdfFileName = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".pdf";
                        var scanQRUrl = _configuration.GetSection("Base:url").Value + "/" + "Certificate" + "/" + pdfFileName;
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(scanQRUrl, QRCodeGenerator.ECCLevel.Q);
                        var qrCode = new QRCoder.QRCode(qrCodeData);
                        Bitmap qrCodeImage = qrCode.GetGraphic(100); // You can adjust the size as needed
                        string base64QR = _util.ImageToBase64(qrCodeImage, ImageFormat.Png);
                        Certificate certificateResponse = await _mediator.Send(new GetCertificatesQueryById(credential));
                        CredentialResponse credentialResponse = JsonConvert.DeserializeObject<CredentialResponse>(certificateResponse.CredentialsJson);
                        var htmlContent = credentialResponse.PrettyVC.Proof;
                        htmlContent = htmlContent.Replace("{{credential.name}}", credentialResponse.Name);
                        htmlContent = htmlContent.Replace("{{subject.name}}", credentialResponse.CredentialSubject.Name);
                        htmlContent = htmlContent.Replace("{{subject.id2}}", credentialResponse.Issuer);
                        htmlContent = htmlContent.Replace("{{qrImage}}", $"data:image/png;base64,{base64QR}");
                        htmlContent = htmlContent.Replace("{{credential.issuanceDate | date: \"%B %d, %Y\"}}", credentialResponse.IssuanceDate.ToString("MMMM d, yyyy"));
                        byte[] pdfBytes = GeneratorPdf(htmlContent);
                        string pdfFilePath = Path.Combine(dynamicFolderPath, pdfFileName);
                        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
                        {
                            await fileStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                            await Task.Delay(1000);

                        }
                    }
                    string zipFilePath = dynamicFolderPath + ".zip";
                    ZipFile.CreateFromDirectory(dynamicFolderPath, zipFilePath);
                    var baseUrl = _configuration.GetSection("Base:url").Value;
                    string subject = string.Empty;
                    string content = string.Empty;
                    if (EnvelopeShareType.CREATE == request.Type) {
                        subject = "Envelope created successfully";
                        content = $"You have created a new envelope " + request.envelopeDTOs.Name + "\n Your envelope includes" + request.envelopeDTOs.Credentials.Count + "Certificate(s) \n You can view them here:: " + baseUrl + zipFilePath.Replace(_hostingEnvironment.WebRootPath + "\\", "/");
                    }
                    if (EnvelopeShareType.SHARE == request.Type)
                    {
                        subject = "Envelope shared successfully";
                        content = $"You have shared a new envelope " + existingEnvelopeName + "\n Your envelope includes" + request.envelopeDTOs.Credentials.Count + "Certificate(s) \n You can view them here:: " + baseUrl + zipFilePath.Replace(_hostingEnvironment.WebRootPath + "\\", "/");
                    }
                    string recipientEmail = request.User.Email;
                    _util.SendSimpleMessage(subject, recipientEmail, content, null, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError ,message: "Unable to create Envelope");
                }
                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: envelopEntity , message: "Envelope created successfully"); ;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new ApiResponse<object>( System.Net.HttpStatusCode.InternalServerError , message: "Unable to create Envelope");
            }
        }

        static void AddFolderToZip(ZipArchive archive, string folderPath, string relativePath)
        {
            foreach (string filePath in Directory.GetFiles(folderPath))
            {
                string entryName = Path.Combine(relativePath, Path.GetFileName(filePath));
                archive.CreateEntryFromFile(filePath, entryName);
            }

            foreach (string subdirectoryPath in Directory.GetDirectories(folderPath))
            {
                string subdirectoryName = Path.GetFileName(subdirectoryPath);
                string entryName = Path.Combine(relativePath, subdirectoryName + "/");
                AddFolderToZip(archive, subdirectoryPath, entryName);
            }
        }

        private byte[] GeneratorPdf(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = DinkToPdf.ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 },
                DocumentTitle = "Generated PDF",
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                Page = "https://www.google.com/",
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 12, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                FooterSettings = { FontSize = 12, Line = true, Right = "© " + DateTime.Now.Year }
            };

            var document = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return _converter.Convert(document);

        }

        private Envelope MapDepartmentDtoToEntity(EnvelopeDTOC envelopeDTO, string path, CommanUser user, EnvelopeShareType Type)
        {
            return new Envelope
            {
                Id = Guid.NewGuid(),
                Name = envelopeDTO.Name,
                Type = Type,
                UserProfileId = user.UserProfile.Id,
                CreatedBy = user.UserProfile.Id,
                Path = path
            };
        }
    }
}