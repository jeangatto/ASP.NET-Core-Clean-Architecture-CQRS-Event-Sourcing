namespace Shop.Core.Extensions;

public static class TExtensions
{
    public static bool IsDefault<T>(this T value) =>
        Equals(value, default(T));
}
