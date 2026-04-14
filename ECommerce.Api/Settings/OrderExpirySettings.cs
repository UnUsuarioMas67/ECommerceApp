namespace ECommerce.Api.Settings;

public class OrderExpirySettings
{
    public int ExpireMinutes { get; set; }
    public int DeleteExpiredAfterHours { get; set; }
    public int CheckExpiryMinutes { get; set; }
}