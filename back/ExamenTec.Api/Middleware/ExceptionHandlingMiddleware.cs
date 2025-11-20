using ExamenTec.Application.Common;
using ExamenTec.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace ExamenTec.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.TraceIdentifier;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, correlationId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        if (!context.Response.Headers.ContainsKey("Content-Type"))
        {
            context.Response.ContentType = "application/json";
        }

        var statusCode = HttpStatusCode.InternalServerError;
        HttpResponse<object> response;
        string logMessage;
        LogLevel logLevel = LogLevel.Error;

        switch (exception)
        {
            case ValidationException validationEx:
                var appValidationMessages = string.Join(", ", validationEx.Errors.SelectMany(e => e.Value));
                response = HttpResponse<object>.BadRequest($"{appValidationMessages}", validationEx.Errors);
                statusCode = HttpStatusCode.BadRequest;
                logMessage = appValidationMessages;
                logLevel = LogLevel.Warning;
                break;

            case BadRequestException badRequestEx:
                response = HttpResponse<object>.BadRequest(badRequestEx.Message);
                statusCode = HttpStatusCode.BadRequest;
                logMessage = badRequestEx.Message;
                logLevel = LogLevel.Warning;
                break;

            case NotFoundException notFoundEx:
                response = HttpResponse<object>.NotFound(notFoundEx.Message);
                statusCode = HttpStatusCode.NotFound;
                logMessage = notFoundEx.Message;
                logLevel = LogLevel.Information;
                break;

            case ConflictException conflictEx:
                response = HttpResponse<object>.Fail(conflictEx.Message, HttpStatusCode.Conflict);
                statusCode = HttpStatusCode.Conflict;
                logMessage = conflictEx.Message;
                logLevel = LogLevel.Warning;
                break;

            case UnauthorizedException unauthorizedEx:
                response = HttpResponse<object>.Unauthorized(unauthorizedEx.Message);
                statusCode = HttpStatusCode.Unauthorized;
                logMessage = unauthorizedEx.Message;
                logLevel = LogLevel.Warning;
                break;

            case ForbiddenException forbiddenEx:
                response = HttpResponse<object>.Forbidden(forbiddenEx.Message);
                statusCode = HttpStatusCode.Forbidden;
                logMessage = forbiddenEx.Message;
                logLevel = LogLevel.Warning;
                break;

            case DbUpdateConcurrencyException concurrencyEx:
                logMessage = $"Concurrency conflict occurred: {concurrencyEx.Message}";
                response = HttpResponse<object>.Fail(
                    "Los datos han sido modificados por otro usuario. Por favor, recargue la página e intente nuevamente.",
                    HttpStatusCode.Conflict);
                statusCode = HttpStatusCode.Conflict;
                logLevel = LogLevel.Warning;
                break;

            case DbUpdateException dbUpdateEx:
                logMessage = $"Database error occurred: {dbUpdateEx.Message}";
                if (dbUpdateEx.InnerException != null)
                {
                    logMessage += $" Inner exception: {dbUpdateEx.InnerException.Message}";
                }

                if (_environment.IsDevelopment())
                {
                    response = HttpResponse<object>.InternalServerError(
                        $"Error de base de datos: {dbUpdateEx.Message}");
                }
                else
                {
                    response = HttpResponse<object>.InternalServerError(
                        "Ocurrió un error al procesar la solicitud. Por favor, intente nuevamente más tarde.");
                }
                statusCode = HttpStatusCode.InternalServerError;
                break;

            default:
                logMessage = $"Unhandled exception: {exception.Message}";
                if (_environment.IsDevelopment())
                {
                    response = HttpResponse<object>.InternalServerError(
                        $"Error interno del servidor: {exception.Message}");
                }
                else
                {
                    response = HttpResponse<object>.InternalServerError(
                        "Ocurrió un error inesperado al procesar la solicitud. Por favor, contacte al administrador del sistema.");
                }
                statusCode = HttpStatusCode.InternalServerError;
                break;
        }

        var corsHeaders = new Dictionary<string, string>();
        foreach (var header in context.Response.Headers)
        {
            if (header.Key.StartsWith("Access-Control-", StringComparison.OrdinalIgnoreCase))
            {
                corsHeaders[header.Key] = string.Join(", ", header.Value.ToArray());
            }
        }

        context.Response.StatusCode = (int)statusCode;

        foreach (var header in corsHeaders)
        {
            if (!context.Response.Headers.ContainsKey(header.Key))
            {
                context.Response.Headers[header.Key] = header.Value;
            }
        }

        LogException(context, exception, logMessage, logLevel, correlationId);

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private void LogException(HttpContext context, Exception exception, string message, LogLevel logLevel, string correlationId)
    {
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var userId = context.User?.Identity?.Name ?? "Anonymous";

        _logger.Log(
            logLevel,
            exception,
            "[CorrelationId: {CorrelationId}] Error processing request: {Message}. " +
            "Path: {Path}, Method: {Method}, User: {User}",
            correlationId,
            message,
            requestPath,
            requestMethod,
            userId);

        if (logLevel == LogLevel.Error && exception != null)
        {
            _logger.LogError(
                exception,
                "[CorrelationId: {CorrelationId}] Stack trace: {StackTrace}",
                correlationId,
                exception.StackTrace);
        }
    }
}

