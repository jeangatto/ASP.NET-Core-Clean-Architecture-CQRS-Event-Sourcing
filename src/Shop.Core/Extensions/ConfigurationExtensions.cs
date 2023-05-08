using System;
using Microsoft.Extensions.Configuration;
using Shop.Core.Abstractions;

namespace Shop.Core.Extensions;

public static class ConfigurationExtensions
{
    private static readonly Action<BinderOptions> ConfigureBinderOptions
        = options => options.BindNonPublicProperties = true;

    public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string configSectionPath)
        where TOptions : IAppOptions
    {
        return configuration.GetSection(configSectionPath).Get<TOptions>(ConfigureBinderOptions);
    }
}