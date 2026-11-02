using System.Net;

namespace Techfin.Case.Application.Validators;

public class Result<T>
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public T? Data { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}
