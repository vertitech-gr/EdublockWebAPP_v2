using DinkToPdf;
using DinkToPdf.Contracts;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Edu_Block_dev.CQRS.Query_Handler.EduEnvelope
{
    public class ShareEnvelopeByIdQueryHandler : IRequestHandler<ShareEnvelopeByIdQuery, EnvelopResponseDTO>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConverter _converter;
        private readonly Util _util;
        private readonly IConfiguration _configuration;


        public ShareEnvelopeByIdQueryHandler(EduBlockDataContext context, IConfiguration configuration, IMediator mediator, IWebHostEnvironment hostingEnvironment, IConverter converter, Util util)
        {
            _context = context;
            _configuration = configuration;
            _mediator = mediator;
            _hostingEnvironment = hostingEnvironment;
            _converter = converter;
            _util = util;

        }
        public async Task<EnvelopResponseDTO> Handle(ShareEnvelopeByIdQuery request, CancellationToken cancellationToken)
        {
            var envelope = await _context.Envelopes
                .FirstOrDefaultAsync(e => e.Id == request.EnvelopeId, cancellationToken);
            var webRootPath = _configuration.GetSection("Base:CertificatePath").Value;

            EnvelopResponseDTO envelopeResult = await _mediator.Send(new EnvelopQueryById(request.EnvelopeId));
            string dynamicFolderName = envelopeResult.Name + DateTime.Now.ToString("yyyyMMddHHmmss");

            dynamicFolderName = "envelopes_" + request.Userid + Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(dynamicFolderName))).Replace("/", "").Replace("+", "");

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
            }
            try
            {
                foreach (var credentialId in request.Credential)
                {
                    Certificate certificateResponse = await _mediator.Send(new GetCertificatesQueryById(credentialId));

                    string pdfFileName = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".pdf";
                    var scanQRUrl = _configuration.GetSection("Base:url").Value + "/" + "Certificate" + "/" + pdfFileName;
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(scanQRUrl, QRCodeGenerator.ECCLevel.Q);
                    var qrCode = new QRCoder.QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(100); // You can adjust the size as needed

                    string base64QR = _util.ImageToBase64(qrCodeImage, ImageFormat.Png);


                    CredentialResponse credentialResponse = JsonConvert.DeserializeObject<CredentialResponse>(certificateResponse.CredentialsJson);
                    var htmlContent = credentialResponse.PrettyVC.Proof;
                    htmlContent = htmlContent.Replace("{{credential.name}}", credentialResponse.Name);
                    //htmlContent = htmlContent.Replace("{{issuer.name}}", credentialResponse.Issuer.Name);
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
                if (request.Email != string.Empty)
                {
                    var baseUrl = _configuration.GetSection("Base:url").Value;
                    {
                        string subject = "Certificate Shared";
                        string content = $"Certificate details:" + baseUrl + "/" + zipFilePath.Replace(_hostingEnvironment.WebRootPath + "\\", "/");
                        string recipientEmail = request.Email;
                        _util.SendSimpleMessage(subject, recipientEmail, content);

                    }

                }

                return envelopeResult;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                return new EnvelopResponseDTO();
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
        private Envelope MapDepartmentDtoToEntity(EnvelopeDTO envelopeDTO)
        {
            return new Envelope
            {
                Name = envelopeDTO.Name,
            };
        }
    }

}
