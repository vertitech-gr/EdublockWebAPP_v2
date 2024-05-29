using System.Text;
using System.Security.Cryptography;
using SendGrid;
using SendGrid.Helpers.Mail;
using RestSharp.Authenticators;
using RestSharp;
using System.Drawing;
using System.Drawing.Imaging;

namespace Edu_Block.DAL
{
    public class Util
    {
        private readonly ILogger<Util> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private static readonly Random random = new Random();
        private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string SpecialCharacters = "!@#$%^&*()_-+=[]{}|\\:;\"'<>,.?/";

        public Util(ILogger<Util> logger, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
        public string GenerateOtp()
        {
            int otpLength = 6;
            StringBuilder otp = new StringBuilder();

            for (int i = 0; i < otpLength; i++)
            {
                otp.Append(random.Next(0, 9).ToString());
            }

            return otp.ToString();
        }

        public string GetDataToHash(Guid id)
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string userSpecificInfo = id.ToString();
            return ComputeSha256Hash($"{timestamp}-{userSpecificInfo}");
        }


        public string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }


        public async Task SendMail(string subject, string to, string htmlContent)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var _from = new EmailAddress(_configuration["SendGrid:SenderEmail"]);
            var _subject = subject;
            var _to = new EmailAddress(to);
            var _htmlContent = htmlContent;
            var msg = MailHelper.CreateSingleEmail(_from, _to, _subject, "",_htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            _logger.LogInformation("send grid reponse =>" + response.ToString());
        }

        public void SendSimpleMessage(string subject, string to, string htmlContent, string? confirmCTA = "Go to Dashboard", string? confirmCTAHref = "https://institution.edublock.bucle.dev")
         {
            string htmlFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "DAL/template", "common-email.html");
            string htmlTemplate = File.ReadAllText(htmlFilePath);
            var options = new RestClientOptions("https://api.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api", _configuration["MailGun:Key"])
            };
            RestClient client = new RestClient(options);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "edu-block.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <EduBlock@edu-block.org>");
            request.AddParameter("to", "<" +  to+ ">");
            request.AddParameter("subject", subject);
            String ctaMessage = "";
            if (confirmCTA != null && confirmCTAHref != null)
            {
                ctaMessage = "<td class=pad style=padding-left:10px;padding-right:10px;padding-top:15px;text-align:center><div align=center class=alignment><!--[if mso]><v:roundrect xmlns:v=urn:schemas-microsoft-com:vml xmlns:w=urn:schemas-microsoft-com:office:word style=height:62px;width:222px;v-text-anchor:middle arcsize=97% stroke=false fillcolor=#1aa19c><w:anchorlock><v:textbox inset=0px,0px,0px,0px><center style=color:#fff;font-family:Tahoma,sans-serif;font-size:16px><![endif]--><div style=\"text-decoration:none;display:inline-block;color:#fff;background-color:#1aa19c;border-radius:60px;width:auto;border-top:0 solid transparent;font-weight:undefined;border-right:0 solid transparent;border-bottom:0 solid transparent;border-left:0 solid transparent;padding-top:15px;padding-bottom:15px;font-family:Montserrat,Trebuchet MS,Lucida Grande,Lucida Sans Unicode,Lucida Sans,Tahoma,sans-serif;font-size:16px;text-align:center;mso-border-alt:none;word-break:keep-all\"><a href=___CONFIRM_CTA_HREF___ style=color:#fff!important><span style=padding-left:30px;padding-right:30px;font-size:16px;display:inline-block;letter-spacing:normal><span style=margin:0;word-break:break-word;line-height:32px><strong>___CONFIRM_CTA___</strong></span></span></a></div><!--[if mso]><![endif]--></div>";
                ctaMessage = ctaMessage.Replace("___CONFIRM_CTA___", confirmCTA);
                ctaMessage = ctaMessage.Replace("___CONFIRM_CTA_HREF___", confirmCTAHref);
            }
            htmlTemplate = htmlTemplate.Replace("___INNER_SUBJECT___", subject);
            htmlTemplate = htmlTemplate.Replace("___BODY___", htmlContent);
            htmlTemplate = htmlTemplate.Replace("___CONFIRM_CTA___", ctaMessage);
            request.AddParameter("html", htmlTemplate);
            request.Method = Method.Post;
            var rr = client.Execute(request);
        }
        public string GenerateAndConvertToSHA(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        public string GenerateRandomPassword(int length)
        {
            const string validChars = "";
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(0, validChars.Length);
                sb.Append(validChars[index]);
            }

          

            return sb.ToString();
        }

        public string GeneratePassword(int length)
        {
            Random random = new Random();
            StringBuilder password = new StringBuilder();

            password.Append(GetRandomCharacter(UppercaseLetters, random));
            password.Append(GetRandomCharacter(LowercaseLetters, random));
            password.Append(GetRandomCharacter(Digits, random));
            password.Append(GetRandomCharacter(SpecialCharacters, random));

            for (int i = 4; i < length; i++)
            {
                string allCharacters = UppercaseLetters + LowercaseLetters + Digits + SpecialCharacters;
                password.Append(GetRandomCharacter(allCharacters, random));
            }

            var output = Shuffle(password.ToString(), random);

            if(output.Length < 8)
            {
                output = GeneratePassword(length);
            }

            return output;
        }

        private char GetRandomCharacter(string characterSet, Random random)
        {
            int index = random.Next(characterSet.Length);
            return characterSet[index];
        }

        private string Shuffle(string str, Random random)
        {
            char[] array = str.ToCharArray();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                char value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }

    }
}