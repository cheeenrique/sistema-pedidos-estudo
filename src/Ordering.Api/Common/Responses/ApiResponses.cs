namespace Ordering.Api.Common.Responses;

public sealed record ApiSuccessResponse<T>(
    bool Success,
    string Message,
    T Data,
    string TraceId);

public sealed record ApiErrorResponse(
    bool Success,
    string ErrorCode,
    string Message,
    int StatusCode,
    IDictionary<string, string[]>? Errors,
    string TraceId);

public static class ApiResponseFactory
{
    public static ApiSuccessResponse<T> Success<T>(T data, string traceId, string message = "Request completed successfully.")
    {
        return new ApiSuccessResponse<T>(true, message, data, traceId);
    }

    public static ApiErrorResponse Error(
        string errorCode,
        string message,
        int statusCode,
        string traceId,
        IDictionary<string, string[]>? errors = null)
    {
        return new ApiErrorResponse(false, errorCode, message, statusCode, errors, traceId);
    }
}
