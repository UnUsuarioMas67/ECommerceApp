using System.Net;

namespace ECommerce.Dashboard.Exceptions;

public class UnexpectedApiResponseException(HttpStatusCode statusCode)
    : HttpRequestException("Unexpected response from API", null, statusCode)
{
    
}