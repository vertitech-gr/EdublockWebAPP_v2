//using System.Text;

//namespace Edu_Block_dev.Authorization;

//public class RequestLoggingMiddleware
//{
//    private readonly RequestDelegate _next;
//    private readonly ILogger<RequestLoggingMiddleware> _logger;

//    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
//    {
//        _next = next;
//        _logger = logger;
//    }

//    public async Task Invoke(HttpContext context)
//    {
//        context.Request.EnableBuffering();
//        string requestBody = await ReadRequestBody(context.Request);
//        _logger.LogInformation($"{requestBody}");
//        context.Request.Body.Seek(0, SeekOrigin.Begin);
//        await _next(context);
//    }

//    private async Task<string> ReadRequestBody(HttpRequest request)
//    {
//        using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
//        {
//            string requestBody = await reader.ReadToEndAsync();
//            return requestBody;
//        }
//    }
//}