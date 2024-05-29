using System.Net.Http.Headers;
using Edu_Block_dev.CQRS.Services.Http;

namespace Edu_Block_dev.CQRS.Services.HttpClients
{
    public class HttpClientFactory : Dictionary<HttpClientEnum, Func<IHttpClient>>, IHttpClientFactory
    {
        public IHttpClient Get(HttpClientEnum type, string token = null)
        {
            var client = this[type].Invoke();
            if (!string.IsNullOrEmpty(token))
                client._client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
