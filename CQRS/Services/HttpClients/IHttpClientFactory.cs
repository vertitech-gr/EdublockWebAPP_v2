
using Edu_Block_dev.CQRS.Services.Http;

namespace Edu_Block_dev.CQRS.Services.HttpClients;

public interface IHttpClientFactory
{
    IHttpClient Get(HttpClientEnum type, string token = null);
}
