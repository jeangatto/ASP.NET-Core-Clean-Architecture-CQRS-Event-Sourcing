using Microsoft.Extensions.Configuration;
using Shop.Core.SharedKernel;

namespace Shop.Core.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Get options from IConfiguration object.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to retrieve.</typeparam>
    /// <param name="configuration">The IConfiguration object.</param>
    /// <returns>The options object.</returns>
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
        where TOptions : class, IAppOptions
    {
        return configuration
            .GetRequiredSection(TOptions.ConfigSectionPath)
            .Get<TOptions>(options => options.BindNonPublicProperties = true);
    }
}