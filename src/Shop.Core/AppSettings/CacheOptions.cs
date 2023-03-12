using Shop.Core.Common;

namespace Shop.Core.AppSettings;

public sealed class CacheOptions : BaseOptions
{
    public const string ConfigSectionPath = nameof(CacheOptions);

    public int AbsoluteExpirationInHours { get; private init; }
    public int SlidingExpirationInSeconds { get; private init; }
}