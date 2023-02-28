using Microsoft.Extensions.Configuration;

namespace Shop.Core.Extensions;

public static class ConfigurationExtensions
{
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string configSectionPath)
        where TOptions : BaseOptions
    {
        return configuration
            .GetSection(configSectionPath)
            .Get<TOptions>(binderOptions => binderOptions.BindNonPublicProperties = true);
    }
}