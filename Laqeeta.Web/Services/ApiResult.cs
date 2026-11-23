using System.Net;

namespace Laqeeta.Web.Services;

public class ApiResult
{
    public HttpStatusCode Status { get; init; }

    public bool Success { get; init; }

    // Friendly (Arabic) error message when Success is false.
    public string? Error { get; init; }
}

public class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }

    public static ApiResult<T> Ok(T data, HttpStatusCode status = HttpStatusCode.OK) =>
        new() { Success = true, Data = data, Status = status };

    public static ApiResult<T> Fail(HttpStatusCode status, string error) =>
        new() { Success = false, Status = status, Error = error };
}
