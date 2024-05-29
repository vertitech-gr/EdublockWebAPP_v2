using System.Text;
using System.Text.Json;
using Edu_Block_dev.Modal.Dock;

namespace Edu_Block_dev.CQRS.Services.HttpClients
{
    public class CrunchClient : IHttpClient
    {
        readonly IConfiguration _config;
        public HttpClient _client { get; set; }
        public CrunchClient(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }
        public AuthenticationHeader AuthenticationHeader =>  new AuthenticationHeader()
        {
            PassName = _config["Crunch:PassName"],
            PassCode = _config["Crunch:PassCode"],
            ClientPassCode = _config["Crunch:ClientPassCode"]
        };
        public async Task<HttpResponseMessage> Get(string extension)
        {
            var response = await _client.GetAsync(extension);
            return response;
        }
        public Task<HttpResponseMessage> Patch<T>(T obj, string path)
        {
            throw new NotImplementedException();
        }
        public async Task<HttpResponseMessage> Post<T>(T obj, string path)
        {
            var jsonContent = JsonSerializer.Serialize(obj);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var result = await _client.PostAsync(path, content);
            result.EnsureSuccessStatusCode();
            return result;            
        }
        public async Task<HttpResponseMessage> PostWithFile(string fullUrl, byte[] fileContent, string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(fileContent), "files", fileName);
            request.Content = content;
            var result = await _client.SendAsync(request);
            return result;
        }
    }
}