using System.Net;

namespace ECommerce.Dashboard.Exceptions;

public class UnexpectedApiResponseException(HttpStatusCode statusCode, IDictionary<string, object>? responseBody = null)
    : HttpRequestException("Unexpected response from API", null, statusCode)
{
    public IDictionary<string, object>? ResponseBody { get; } = responseBody;
}