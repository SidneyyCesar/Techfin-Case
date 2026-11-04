using System.Net;

namespace Techfin.Shared;

public class Result<T>
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public T? Data { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}
