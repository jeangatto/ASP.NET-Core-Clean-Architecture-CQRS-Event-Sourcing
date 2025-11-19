using Microsoft.Extensions.Configuration;
using Shop.Core.SharedKernel;

namespace Shop.Core.Extensions;

public static class ConfigurationExtensions
{
    extension<TOptions>(IConfiguration configuration) where TOptions : class, IAppOptions
    {
        /// <summary>
        /// Get options from IConfiguration object.
        /// </summary>
        /// <returns>The options object.</returns>
        public TOptions GetOptions()
        {
            return configuration
                .GetRequiredSection(TOptions.ConfigSectionPath)
                .Get<TOptions>(options => options.BindNonPublicProperties = true);
        }
    }
}