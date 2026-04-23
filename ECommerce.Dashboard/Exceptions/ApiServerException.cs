using System.Net;

namespace ECommerce.Dashboard.Exceptions;

public class ApiServerException(HttpStatusCode statusCode)
    : HttpRequestException("Request failed due to API server error", null, statusCode);