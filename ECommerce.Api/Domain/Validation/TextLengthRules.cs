namespace ECommerce.Api.Domain.Validation;

public static class TextLengthRules
{
    public const int LongText = 1000;
    public const int ShortText = 100;
    public const int Sku = 50;
    public const int Name = 50;
    public const int ShortName = 20;
    public const int Email = 320;
    public const int PhoneNumber = 25;
    public const int PostalCode = 10;
    public const int PasswordHash = 255;
    public const int PasswordMinLenght = 8;
}