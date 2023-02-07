using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Shop.Application;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddProfiles(this IServiceCollection services)
    {
        return services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}