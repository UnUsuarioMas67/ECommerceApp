namespace ECommerce.Dashboard.DTOs.Error;

public class ApiErrorResponse
{
    public string ErrorType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public IDictionary<string, object?> Details { get; set; } = new Dictionary<string, object?>();
}