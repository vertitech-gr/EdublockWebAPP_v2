using System.Net;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Error { get; set; }
    public string Message { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public ApiResponse(HttpStatusCode statusCode, string message)
    {
        StatusCode = statusCode;
        Success = statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created;
        Message = message;
    }

    public ApiResponse(HttpStatusCode statusCode, T data, string message = null)
    {
        StatusCode = statusCode;
        Success = statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created;
        Data = data;
        Message = message;
    }

    public ApiResponse(HttpStatusCode statusCode, string error, string message = null)
    {
        StatusCode = statusCode;
        Success = false;
        Error = error;
        Message = message;
    }
}