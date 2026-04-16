namespace ECommerce.Api.Settings;

public class OrderExpirySettings
{
    public int ExpireMinutes { get; set; } = 15;
    public int DeleteExpiredAfterHours { get; set; } = 24;
    public int CheckExpiryMinutes { get; set; } = 5;
}