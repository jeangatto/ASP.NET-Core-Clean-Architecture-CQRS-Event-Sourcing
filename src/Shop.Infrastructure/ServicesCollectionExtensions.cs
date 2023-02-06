using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Shop.Core.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Behaviors;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Data.Context;
using Shop.Infrastructure.Data.Events;
using Shop.Infrastructure.Data.Events.Mappings;
using Shop.Infrastructure.Data.Repositories;

namespace Shop.Infrastructure;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // MediatR Pipelines
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ShopContext>();
        services.AddScoped<EventContext>();

        ConfigureMongoDB();

        return services;
    }

    private static void ConfigureMongoDB()
    {
        // Passo 1º: Configurar o tipo do serializador de Guid.
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));

        // Passo 2º: Configurar as convenções, assim será aplicado para todos os mapeamentos.
        // REF: https://mongodb.github.io/mongo-csharp-driver/2.0/reference/bson/mapping/conventions/
        ConventionRegistry.Register("Conventions", new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreExtraElementsConvention(true),
            new IgnoreIfNullConvention(true)
        }, _ => true);

        // Passo 3º: Registrar as configurações dos mapeamento das classes.
        // É recomendável registrar todos os mapeamentos antes de inicializar a conexão com o MongoDb
        // REF: https://mongodb.github.io/mongo-csharp-driver/2.0/reference/bson/mapping/
        BaseDomainEventMap.Configure();
        StoredEventMap.Configure();
    }
}