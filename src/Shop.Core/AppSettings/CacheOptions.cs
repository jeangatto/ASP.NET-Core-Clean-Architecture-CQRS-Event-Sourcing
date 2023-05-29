using System.Diagnostics.CodeAnalysis;
using Shop.Core.Shared;

namespace Shop.Core.AppSettings;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class CacheOptions : IAppOptions
{
    public const string ConfigSectionPath = nameof(CacheOptions);

    public int AbsoluteExpirationInHours { get; private init; }
    public int SlidingExpirationInSeconds { get; private init; }
}