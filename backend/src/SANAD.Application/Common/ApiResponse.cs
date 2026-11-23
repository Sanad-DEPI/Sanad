namespace SANAD.Application.Common;

/// <summary>
/// Consistent envelope for API responses.
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Succeeded { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    public static ApiResponse<T> Ok(T data) => new() { Succeeded = true, Data = data };

    public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null) =>
        new() { Succeeded = false, Message = message, Errors = errors ?? [] };
}
