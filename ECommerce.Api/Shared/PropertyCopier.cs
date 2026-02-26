namespace ECommerce.Api.Shared;

public static class PropertyCopier
{
    public static T Mirror<T>(T source, T destination) where T : class
    {
        var sourceProperties = source.GetType().GetProperties();
        var destinationProperties = destination.GetType().GetProperties();

        foreach (var sourceProperty in sourceProperties)
        {
            foreach (var destinationProperty in destinationProperties)
            {
                if (sourceProperty.Name == destinationProperty.Name 
                    && sourceProperty.PropertyType == destinationProperty.PropertyType
                    && destinationProperty.CanWrite)
                {
                    destinationProperty.SetValue(destination, sourceProperty.GetValue(source));
                    break;
                }
            }
        }

        return destination;
    }

    public static T GetCopy<T>(T source) where T : class
    {
        return Mirror(source, (T)Activator.CreateInstance(typeof(T))!);
    }
}