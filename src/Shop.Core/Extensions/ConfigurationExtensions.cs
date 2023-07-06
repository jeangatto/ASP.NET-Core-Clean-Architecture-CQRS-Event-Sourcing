using System;
using Microsoft.Extensions.Configuration;
using Shop.Core.SharedKernel;

namespace Shop.Core.Extensions;

public static class ConfigurationExtensions
{
    private static readonly Action<BinderOptions> ConfigureBinderOptions
        = options => options.BindNonPublicProperties = true;

    /// <summary>
    /// Get options from IConfiguration object.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to retrieve.</typeparam>
    /// <param name="configuration">The IConfiguration object.</param>
    /// <param name="configSectionPath">The configuration section path.</param>
    /// <returns>The options object.</returns>
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string configSectionPath)
        where TOptions : class, IAppOptions =>
        configuration.GetRequiredSection(configSectionPath).Get<TOptions>(ConfigureBinderOptions);
}