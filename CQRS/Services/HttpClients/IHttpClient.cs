using Edu_Block_dev.Modal.Dock;

namespace Edu_Block_dev.CQRS.Services.HttpClients;

public interface IHttpClient
{
    HttpClient _client { get; set; }
    Task<HttpResponseMessage> Post<T>(T obj, string path);
    Task<HttpResponseMessage> PostWithFile(string fullUrl, byte[] fileContent, string fileName);
    Task<HttpResponseMessage> Patch<T>(T obj, string path);
    Task<HttpResponseMessage> Get(string extension);
    AuthenticationHeader AuthenticationHeader { get; } 
}
