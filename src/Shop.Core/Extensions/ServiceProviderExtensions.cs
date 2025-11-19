using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shop.Core.SharedKernel;

namespace Shop.Core.Extensions;

public static class ServiceProviderExtensions
{
    extension<TOptions>(IServiceProvider serviceProvider) where TOptions : class, IAppOptions
    {
        /// <summary>
        /// Get options from the service provider.
        /// </summary>
        /// <returns>The options.</returns>
        public TOptions GetOptions() => serviceProvider.GetService<IOptions<TOptions>>()?.Value;
    }
}