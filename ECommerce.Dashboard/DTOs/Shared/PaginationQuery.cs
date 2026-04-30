namespace ECommerce.Dashboard.DTOs.Shared;

public record PaginationQuery(int Limit = 20, int Page = 1);