//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Net;
//using System.Text;
//using System.Transactions;
//using DinkToPdf;
//using DinkToPdf.Contracts;
//using Edu_Block.DAL;
//using Edu_Block.DAL.EF;
//using Edu_Block_dev.CQRS.Commands.UserCommand;
//using Edu_Block_dev.Modal.Dock;
//using Edu_Block_dev.Modal.DTO;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using QRCoder;

//namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
//{
//    public class IssueCredentialCommandHandler : IRequestHandler<IssuerCredentialCommand, ApiResponse<object>>
//    {
//        private readonly EduBlockDataContext _context;
//        private readonly IRepository<Certificate> _certificateRepository;
//        private readonly IWebHostEnvironment _hostingEnvironment;
//        private readonly IConfiguration _configuration;
//        private readonly IMediator _mediator;
//        private readonly Util _util;
//        private readonly IConverter _converter; // Injected DinkToPdf converter

//        public IssueCredentialCommandHandler(
//            EduBlockDataContext context,
//            IRepository<Certificate> certificateRepository,
//            IWebHostEnvironment hostingEnvironment,
//            IConfiguration configuration,
//            IMediator mediator,
//            Util util,
//            IConverter converter) // Inject IConverter
//        {
//            _context = context;
//            _certificateRepository = certificateRepository;
//            _hostingEnvironment = hostingEnvironment;
//            _configuration = configuration;
//            _mediator = mediator;
//            _util = util;
//            _converter = converter;
//        }

//        public async Task<ApiResponse<object>> Handle(IssuerCredentialCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//                {
//                    var client = new HttpClient();
//                    var apiKey = _configuration.GetSection("Dock:ApiKey").Value;

//                    var payload = new RegistryPayloadDTO
//                    {
//                        AddOnly = false,
//                        Policy = new List<string> { request._issuerCredentialRequest.Issuer },
//                        RegistryType = "StatusList2021Entry"
//                    };

//                    string registryJSONPayload = JsonConvert.SerializeObject(payload, Formatting.Indented);
//                    var registryResponse = await SendRegistryHttpRequest(client, registryJSONPayload, apiKey);

//                    RegistryResponse registryResponseData = JsonConvert.DeserializeObject<RegistryResponse>(registryResponse);

//                    var subjectPayload = JsonConvert.DeserializeObject(request._issuerCredentialRequest.Subject.ToString());
//                    var dockCredentialDTO = new DockCredentialDTO()
//                    {
//                        Schema = request._issuerCredentialRequest.Schema,
//                        Name = request._issuerCredentialRequest.Name,
//                        Type = new List<string>() { "VerifiableCredential", request._issuerCredentialRequest.Name },
//                        Issuer = request._issuerCredentialRequest.Issuer,
//                        IssuanceDate = request._issuerCredentialRequest.IssuanceDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
//                        Subject = subjectPayload,
//                        Status = registryResponseData.Data.Id
//                    };
//                    DockIssuerCredentialDTO dockIssuerCredentialDTO = new DockIssuerCredentialDTO()
//                    {
//                        Anchor = false,
//                        Password = request._issuerCredentialRequest.Password,
//                        Persist = true,
//                        Template = request._issuerCredentialRequest.Template,
//                        Credential = dockCredentialDTO,
//                        RecipientEmail = request._issuerCredentialRequest.RecipientEmail,
//                        EmailMessage = "",
//                        Distribute = true
//                    };

//                    var jsonPayload = JsonConvert.SerializeObject(dockIssuerCredentialDTO);
//                    var response = await SendHttpRequest(client, jsonPayload, apiKey);
//                    CredentialResponse credentialResponse = JsonConvert.DeserializeObject<CredentialResponse>(response);

//                    if (credentialResponse != null && credentialResponse.PrettyVC != null && credentialResponse.PrettyVC.Proof != null)
//                    {
//                        var htmlContent = credentialResponse.PrettyVC.Proof;
//                        JObject subjectPayloadObject = subjectPayload as JObject;

//                        if (subjectPayloadObject != null)
//                        {
//                            foreach (var property in subjectPayloadObject.Properties())
//                            {
//                                htmlContent = htmlContent.Replace("{{subject." + property.Name + "}}", property.Value.ToString());
//                            }
//                        }

//                        // Retrieve University and related details
//                        UniversityDockDetialsDTO universityDetails = RetrieveUniversityDetails(credentialResponse.Issuer);

//                        string pdfFileName = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".pdf";
//                        var scanQRUrl = _configuration.GetSection("Base:url").Value + "/" + "Certificate" + "/" + pdfFileName;
//                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
//                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(scanQRUrl, QRCodeGenerator.ECCLevel.Q);
//                        var qrCode = new QRCoder.QRCode(qrCodeData);
//                        Bitmap qrCodeImage = qrCode.GetGraphic(100); // You can adjust the size as neededs
//                        string base64QR = _util.ImageToBase64(qrCodeImage, ImageFormat.Png);

//                        htmlContent = htmlContent.Replace("{{credential.id}}", credentialResponse.Id);
//                        htmlContent = htmlContent.Replace("{{subject.name}}", request._issuerCredentialRequest.Name);
//                        htmlContent = htmlContent.Replace("{{subject.email}}", request._issuerCredentialRequest.Email);
//                        htmlContent = htmlContent.Replace("{{issuer.id}}", universityDetails.University.Id.ToString());
//                        htmlContent = htmlContent.Replace("{{subject.rollNo}}", request._issuerCredentialRequest.RollNo);
//                        htmlContent = htmlContent.Replace("{{issuer.name}}", universityDetails.University.Name);
//                        htmlContent = htmlContent.Replace("{{issuer.description}}", universityDetails.UserProfile.Description);
//                        htmlContent = htmlContent.Replace("{{qrImage}}", $"data:image/png;base64,{base64QR}");
//                        htmlContent = htmlContent.Replace("{{issuer.id}}", universityDetails.University.Id.ToString());
//                        htmlContent = htmlContent.Replace("{{credential.issuanceDate | date: \"%B %d, %Y\"}}", credentialResponse.IssuanceDate.ToString("MMMM d, yyyy"));
//                        htmlContent = htmlContent.Replace("{{credential.expirationDate | date: \"%B %d, %Y\"}}", request._issuerCredentialRequest.ExpireDate.ToString("MMMM d, yyyy"));

//                        byte[] pdfBytes = this.GeneratorPdf(htmlContent);
//                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Certificate", pdfFileName);
//                        File.WriteAllBytes(filePath, pdfBytes);
//                        var fileStreamResult = new FileStreamResult(new FileStream(filePath, FileMode.Open), "application/Certificate")
//                        {
//                            FileDownloadName = pdfFileName
//                        };

//                        //Generate PDF and QR code
//                        //string pdfFileName = GenerateCertificate(htmlContent, universityDetails);

//                        // Save certificate details
//                        await SaveCertificateDetails(request, response, credentialResponse.Id, registryResponseData.Data.Id, pdfFileName, scanQRUrl);

//                        // Commit transaction if all operations succeed
//                        scope.Complete();

//                        return new ApiResponse<object>(HttpStatusCode.OK, data: response.ToString(), message: "Certificate created successfully");
//                    }

//                    return new ApiResponse<object>(HttpStatusCode.InternalServerError, data: string.Empty, message: "Unable to create certificate");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"An error occurred: {ex.Message}");
//                return new ApiResponse<object>(HttpStatusCode.InternalServerError, data: string.Empty, message: ex.Message);
//            }
//        }

//        private async Task<string> SendHttpRequest(HttpClient client, string jsonPayload, string apiKey)
//        {
//            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-testnet.dock.io/credentials");
//            request.Headers.Add("dock-api-token", apiKey);
//            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
//            var result = await client.SendAsync(request);
//            return await result.Content.ReadAsStringAsync();
//        }

//        private async Task<string> SendRegistryHttpRequest(HttpClient client, string jsonPayload, string apiKey)
//        {
//            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-testnet.dock.io/registries");
//            request.Headers.Add("dock-api-token", apiKey);
//            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
//            var result = await client.SendAsync(request);
//            return await result.Content.ReadAsStringAsync();
//        }

//        private UniversityDockDetialsDTO RetrieveUniversityDetails(string issuerDID)
//        {
//            return _context.Universities
//                .Join(_context.UserProfiles, u => u.Id, up => up.UserID, (u, up) => new {
//                    University = u,
//                    UserProfile = up
//                })
//                .Join(_context.DockIoDIDs, uup => uup.UserProfile.Id, did => did.UserProfileId, (uup, did) => new UniversityDockDetialsDTO
//                {
//                    University = uup.University,
//                    UserProfile = uup.UserProfile,
//                    DockIoDID = did
//                })
//                .FirstOrDefault(condition => condition.DockIoDID.DID == issuerDID);
//        }

//        public byte[] GeneratorPdf(string htmlContent)
//        {
//            var globalSettings = new GlobalSettings
//            {
//                ColorMode = DinkToPdf.ColorMode.Color,
//                Orientation = Orientation.Landscape,
//                PaperSize = PaperKind.A4,
//                Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 },
//                DocumentTitle = "Generated PDF",
//            };

//            var objectSettings = new ObjectSettings
//            {
//                PagesCount = true,
//                HtmlContent = htmlContent,
//                Page = "https://www.google.com/",
//                WebSettings = { DefaultEncoding = "utf-8" },
//                HeaderSettings = { FontSize = 12, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
//                FooterSettings = { FontSize = 12, Line = true, Right = "© " + DateTime.Now.Year }
//            };

//            var document = new HtmlToPdfDocument()
//            {
//                GlobalSettings = globalSettings,
//                Objects = { objectSettings }
//            };

//            return _converter.Convert(document);
//        }


//        //private string GenerateCertificate(string htmlContent, UniversityDockDetialsDTO universityDetails)
//        //{
//        //    var globalSettings = new GlobalSettings
//        //    {
//        //        ColorMode = DinkToPdf.ColorMode.Color,
//        //        Orientation = Orientation.Landscape,
//        //        PaperSize = PaperKind.A4,
//        //        Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 },
//        //        DocumentTitle = "Generated PDF",
//        //    };

//        //    var objectSettings = new ObjectSettings
//        //    {
//        //        PagesCount = true,
//        //        HtmlContent = htmlContent,
//        //        WebSettings = { DefaultEncoding = "utf-8" },
//        //        HeaderSettings = { FontSize = 12, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
//        //        FooterSettings = { FontSize = 12, Line = true, Right = $"© {DateTime.Now.Year}" }
//        //    };

//        //    var document = new HtmlToPdfDocument()
//        //    {
//        //        GlobalSettings = globalSettings,
//        //        Objects = { objectSettings }
//        //    };

//        //    byte[] pdfBytes = _converter.Convert(document);

//        //    // Save PDF to a file (You can adjust the file path and name as needed)
//        //    string pdfFileName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.pdf";
//        //    string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Certificate", pdfFileName);
//        //    File.WriteAllBytes(filePath, pdfBytes);

//        //    return pdfFileName;
//        //}

//        private async Task SaveCertificateDetails(IssuerCredentialCommand request, string response, string certificateId, string registryId, string pdfFileName, string scanQRUrl)
//        {
//            var certificate = new Certificate()
//            {
//                Status = true,
//                RegistryID = registryId,
//                CertificateID = certificateId,
//                UserProfileId = request._issuerCredentialRequest.userProfileID,
//                Issuer = request._issuerCredentialRequest.Issuer,
//                CredentialsJson = response.ToString(),
//                DegreeName = request._issuerCredentialRequest.DegreeName,
//                DegreeType = request._issuerCredentialRequest.DegreeType,
//                Password = request._issuerCredentialRequest.Password,
//                DegreeAwardedDate = DateTime.UtcNow,
//                DateOfBirth = request._issuerCredentialRequest.DateOfBirth,
//                IssuanceDate = request._issuerCredentialRequest.IssuanceDate,
//                ExpireDate = request._issuerCredentialRequest.ExpireDate,
//                StartDate = request._issuerCredentialRequest.StartDate,
//                EndDate = request._issuerCredentialRequest.EndDate,
//                FileName = pdfFileName,
//                Path = "Certificate",
//                CreatedBy = request._issuerCredentialRequest.userProfileID,

//            };

//            await _certificateRepository.AddAsync(certificate);

//            // Send notification or email to user
//            SendNotification(request._issuerCredentialRequest.Name, request._issuerCredentialRequest.Email, pdfFileName, scanQRUrl);
//        }

//        private void SendNotification(string name, string email, string pdfFileName, string scanQRUrl)
//        {
//            var subject = "Certificate on EduBlock";
//            var content = $"<p>Dear {name}, We welcome you on Edublock Platform</p>" +
//                $"<br>" +
//                $"<span> User Id : <strong>{email}</strong></span>" +
//                $"<br>" +
//                $"<span> Click on this link : <strong>{scanQRUrl}</span>";

//            _util.SendSimpleMessage(subject, email, content, null , null);
//        }
//    }
//}


using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Text;
using System.Transactions;
using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Edu_Block_dev.Modal.Dock;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using QRCoder;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class IssueCredentialCommandHandler : IRequestHandler<IssuerCredentialCommand, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _context;
        private readonly IRepository<Certificate> _certificateRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly Util _util;

        public IssueCredentialCommandHandler(
            EduBlockDataContext context,
            IRepository<Certificate> certificateRepository,
            IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration,
            IMediator mediator,
            Util util)
        {
            _context = context;
            _certificateRepository = certificateRepository;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _mediator = mediator;
            _util = util;
        }

        public async Task<ApiResponse<object>> Handle(IssuerCredentialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var client = new HttpClient();
                    var apiKey = _configuration.GetSection("Dock:ApiKey").Value;

                    var payload = new RegistryPayloadDTO
                    {
                        AddOnly = false,
                        Policy = new List<string> { request._issuerCredentialRequest.Issuer },
                        RegistryType = "StatusList2021Entry"
                    };

                    string registryJSONPayload = JsonConvert.SerializeObject(payload, Formatting.Indented);
                    var registryResponse = await SendRegistryHttpRequest(client, registryJSONPayload, apiKey);

                    RegistryResponse registryResponseData = JsonConvert.DeserializeObject<RegistryResponse>(registryResponse);

                    var subjectPayload = JsonConvert.DeserializeObject(request._issuerCredentialRequest.Subject.ToString());
                    var dockCredentialDTO = new DockCredentialDTO()
                    {
                        Schema = request._issuerCredentialRequest.Schema,
                        Name = request._issuerCredentialRequest.Name,
                        Type = new List<string>() { "VerifiableCredential", request._issuerCredentialRequest.Name },
                        Issuer = request._issuerCredentialRequest.Issuer,
                        IssuanceDate = request._issuerCredentialRequest.IssuanceDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Subject = subjectPayload,
                        Status = registryResponseData.Data.Id
                    };
                    DockIssuerCredentialDTO dockIssuerCredentialDTO = new DockIssuerCredentialDTO()
                    {
                        Anchor = false,
                        Password = request._issuerCredentialRequest.Password,
                        Persist = true,
                        Template = request._issuerCredentialRequest.Template,
                        Credential = dockCredentialDTO,
                        RecipientEmail = request._issuerCredentialRequest.RecipientEmail,
                        EmailMessage = "",
                        Distribute = true
                    };

                    var jsonPayload = JsonConvert.SerializeObject(dockIssuerCredentialDTO);
                    var response = await SendHttpRequest(client, jsonPayload, apiKey);
                    CredentialResponse credentialResponse = JsonConvert.DeserializeObject<CredentialResponse>(response);

                    if (credentialResponse != null && credentialResponse.PrettyVC != null && credentialResponse.PrettyVC.Proof != null)
                    {
                        var htmlContent = credentialResponse.PrettyVC.Proof;
                        JObject subjectPayloadObject = subjectPayload as JObject;

                        if (subjectPayloadObject != null)
                        {
                            foreach (var property in subjectPayloadObject.Properties())
                            {
                                htmlContent = htmlContent.Replace("{{subject." + property.Name + "}}", property.Value.ToString());
                            }
                        }

                        // Retrieve University and related details
                        UniversityDockDetialsDTO universityDetails = RetrieveUniversityDetails(credentialResponse.Issuer);

                        string pdfFileName = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".pdf";
                        var scanQRUrl = _configuration.GetSection("Base:url").Value + "/" + "Certificate" + "/" + pdfFileName;
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(scanQRUrl, QRCodeGenerator.ECCLevel.Q);
                        var qrCode = new QRCoder.QRCode(qrCodeData);
                        Bitmap qrCodeImage = qrCode.GetGraphic(100); // You can adjust the size as neededs
                        string base64QR = _util.ImageToBase64(qrCodeImage, ImageFormat.Png);

                        htmlContent = htmlContent.Replace("{{credential.id}}", credentialResponse.Id);
                        htmlContent = htmlContent.Replace("{{subject.name}}", request._issuerCredentialRequest.Name);
                        htmlContent = htmlContent.Replace("{{subject.email}}", request._issuerCredentialRequest.Email);
                        htmlContent = htmlContent.Replace("{{issuer.id}}", universityDetails.University.Id.ToString());
                        htmlContent = htmlContent.Replace("{{subject.rollNo}}", request._issuerCredentialRequest.RollNo);
                        htmlContent = htmlContent.Replace("{{issuer.name}}", universityDetails.University.Name);
                        htmlContent = htmlContent.Replace("{{issuer.description}}", universityDetails.UserProfile.Description);
                        htmlContent = htmlContent.Replace("{{qrImage}}", $"data:image/png;base64,{base64QR}");
                        htmlContent = htmlContent.Replace("{{issuer.id}}", universityDetails.University.Id.ToString());
                        htmlContent = htmlContent.Replace("{{credential.issuanceDate | date: \"%B %d, %Y\"}}", credentialResponse.IssuanceDate.ToString("MMMM d, yyyy"));
                        htmlContent = htmlContent.Replace("{{credential.expirationDate | date: \"%B %d, %Y\"}}", request._issuerCredentialRequest.ExpireDate.ToString("MMMM d, yyyy"));

                        byte[] pdfBytes = await GeneratePdfWithPuppeteer(htmlContent);
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Certificate", pdfFileName);
                        await File.WriteAllBytesAsync(filePath, pdfBytes);
                        var fileStreamResult = new FileStreamResult(new FileStream(filePath, FileMode.Open), "application/Certificate")
                        {
                            FileDownloadName = pdfFileName
                        };

                        // Save certificate details
                        await SaveCertificateDetails(request, response, credentialResponse.Id, registryResponseData.Data.Id, pdfFileName, scanQRUrl);

                        // Commit transaction if all operations succeed
                        scope.Complete();

                        return new ApiResponse<object>(HttpStatusCode.OK, data: response.ToString(), message: "Certificate created successfully");
                    }

                    return new ApiResponse<object>(HttpStatusCode.InternalServerError, data: string.Empty, message: "Unable to create certificate");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new ApiResponse<object>(HttpStatusCode.InternalServerError, data: string.Empty, message: ex.Message);
            }
        }

        private async Task<string> SendHttpRequest(HttpClient client, string jsonPayload, string apiKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-testnet.dock.io/credentials");
            request.Headers.Add("dock-api-token", apiKey);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var result = await client.SendAsync(request);
            return await result.Content.ReadAsStringAsync();
        }

        private async Task<string> SendRegistryHttpRequest(HttpClient client, string jsonPayload, string apiKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-testnet.dock.io/registries");
            request.Headers.Add("dock-api-token", apiKey);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var result = await client.SendAsync(request);
            return await result.Content.ReadAsStringAsync();
        }

        private UniversityDockDetialsDTO RetrieveUniversityDetails(string issuerDID)
        {
            return _context.Universities
                .Join(_context.UserProfiles, u => u.Id, up => up.UserID, (u, up) => new {
                    University = u,
                    UserProfile = up
                })
                .Join(_context.DockIoDIDs, uup => uup.UserProfile.Id, did => did.UserProfileId, (uup, did) => new UniversityDockDetialsDTO
                {
                    University = uup.University,
                    UserProfile = uup.UserProfile,
                    DockIoDID = did
                })
                .FirstOrDefault(condition => condition.DockIoDID.DID == issuerDID);
        }

        public async Task<byte[]> GeneratePdfWithPuppeteer(string htmlContent)
        {
            await new BrowserFetcher().DownloadAsync();
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);
            var pdfOptions = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true
            };
            var pdfStream = await page.PdfStreamAsync(pdfOptions);
            await browser.CloseAsync();

            using (var memoryStream = new MemoryStream())
            {
                await pdfStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private async Task SaveCertificateDetails(IssuerCredentialCommand request, string response, string certificateId, string registryId, string pdfFileName, string scanQRUrl)
        {
            var certificate = new Certificate()
            {

                Status = true,
                RegistryID = registryId,
                CertificateID = certificateId,
                UserProfileId = request._issuerCredentialRequest.userProfileID,
                Issuer = request._issuerCredentialRequest.Issuer,
                CredentialsJson = response.ToString(),
                DegreeName = request._issuerCredentialRequest.DegreeName,
                DegreeType = request._issuerCredentialRequest.DegreeType,
                Password = request._issuerCredentialRequest.Password,
                DegreeAwardedDate = DateTime.UtcNow,
                DateOfBirth = request._issuerCredentialRequest.DateOfBirth,
                IssuanceDate = request._issuerCredentialRequest.IssuanceDate,
                ExpireDate = request._issuerCredentialRequest.ExpireDate,
                StartDate = request._issuerCredentialRequest.StartDate,
                EndDate = request._issuerCredentialRequest.EndDate,
                FileName = pdfFileName,
                Path = "Certificate",
                CreatedBy = request._issuerCredentialRequest.userProfileID,
            };

            await _certificateRepository.AddAsync(certificate);
        }
    }
}

