using System.Diagnostics.CodeAnalysis;
using Shop.Core.SharedKernel;

namespace Shop.Core.AppSettings;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class CacheOptions : IAppOptions
{
    static string IAppOptions.ConfigSectionPath => nameof(CacheOptions);

    public int AbsoluteExpirationInHours { get; private init; }
    public int SlidingExpirationInSeconds { get; private init; }
}