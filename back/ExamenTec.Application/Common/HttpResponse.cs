using System.Net;

namespace ExamenTec.Application.Common;

public class HttpResponse<T> where T : class
{
    public T? Result { get; private set; }
    public string Message { get; private set; }
    public HttpStatusCode StatusCode { get; private set; }
    public bool IsError => (int)StatusCode >= (int)HttpStatusCode.BadRequest;

    private HttpResponse(T? result, string? message, HttpStatusCode statusCode, object? errors = null)
    {
        Result = result;
        Message = message ?? string.Empty;
        StatusCode = statusCode;
    }

    public HttpResponse()
    {
        Message = string.Empty;
        StatusCode = HttpStatusCode.OK;
    }

    public static HttpResponse<T> Ok(string? message = null)
    {
        return new HttpResponse<T>(null, message, HttpStatusCode.OK);
    }

    public static HttpResponse<T> Ok(T data, string? message = null)
    {
        return new HttpResponse<T>(data, message, HttpStatusCode.OK);
    }

    public static HttpResponse<T> Created(T data, string? message = null)
    {
        return new HttpResponse<T>(data, message, HttpStatusCode.Created);
    }

    public static HttpResponse<T> Fail(string message, HttpStatusCode code = HttpStatusCode.BadRequest)
    {
        return new HttpResponse<T>(null, message, code, new List<string> { message });
    }

    public static HttpResponse<T> Fail(string message, IEnumerable<string> errors, HttpStatusCode code = HttpStatusCode.Conflict)
    {
        return new HttpResponse<T>(null, message, code, errors);
    }

    public static HttpResponse<T> Fail(string message, IEnumerable<string> errors, T? data, HttpStatusCode code = HttpStatusCode.Conflict)
    {
        return new HttpResponse<T>(data, message, code, errors);
    }

    public static HttpResponse<T> BadRequest(string message)
    {
        return new HttpResponse<T>(null, message, HttpStatusCode.BadRequest);
    }

    public static HttpResponse<T> BadRequest(string message, object? errors)
    {
        return new HttpResponse<T>(null, message, HttpStatusCode.BadRequest, errors);
    }

    public static HttpResponse<T> NotFound(string message)
    {
        return new HttpResponse<T>(null, message, HttpStatusCode.NotFound);
    }

    public static HttpResponse<T> Unauthorized(string message)
    {
        return new HttpResponse<T>(null, message, HttpStatusCode.Unauthorized);
    }

    public static HttpResponse<T> Forbidden(string message)
    {
        return new HttpResponse<T>(null, message, HttpStatusCode.Forbidden);
    }

    public static HttpResponse<T> InternalServerError(string message)
    {
        return new HttpResponse<T>(null, message, HttpStatusCode.InternalServerError);
    }
}

