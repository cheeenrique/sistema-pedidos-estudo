using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Ordering.Api.Common.Responses;

namespace Ordering.Api.Common.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message, errors) = MapException(exception);

        _logger.LogError(
            exception,
            "Unhandled exception. TraceId: {TraceId}, ErrorCode: {ErrorCode}, StatusCode: {StatusCode}",
            context.TraceIdentifier,
            errorCode,
            statusCode);

        var response = ApiResponseFactory.Error(
            errorCode,
            message,
            statusCode,
            context.TraceIdentifier,
            errors);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private static (int StatusCode, string ErrorCode, string Message, IDictionary<string, string[]>? Errors) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "ValidationError",
                "One or more validation errors occurred.",
                new Dictionary<string, string[]> { ["validation"] = [validationException.Message] }),

            ArgumentException argumentException => (
                StatusCodes.Status400BadRequest,
                "BadRequest",
                argumentException.Message,
                null),

            InvalidOperationException invalidOperationException => (
                StatusCodes.Status400BadRequest,
                "BusinessRuleViolation",
                invalidOperationException.Message,
                null),

            KeyNotFoundException keyNotFoundException => (
                StatusCodes.Status404NotFound,
                "NotFound",
                keyNotFoundException.Message,
                null),

            _ => (
                StatusCodes.Status500InternalServerError,
                "InternalServerError",
                "An unexpected error occurred.",
                null)
        };
    }
}
