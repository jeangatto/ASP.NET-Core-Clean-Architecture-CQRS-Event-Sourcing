using System.Reflection;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Shop.Query.Abstractions;
using Shop.Query.Data.Context;
using Shop.Query.Data.Mappings;
using Shop.Query.Data.Repositories;
using Shop.Query.Data.Repositories.Abstractions;

namespace Shop.Query.Extensions;

public static class ServicesCollectionExtensions
{
    public static void AddQuery(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(executingAssembly);

        // Add AutoMapper as a singleton instance
        services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(cfg => cfg.AddMaps(executingAssembly))));

        // Use AddValidatorsFromAssembly instead of AddValidatorsFromAssemblies
        services.AddValidatorsFromAssembly(executingAssembly);

        services.AddScoped<IReadDbContext, ReadDbContext>();
        services.AddScoped<ICustomerReadOnlyRepository, CustomerReadOnlyRepository>();

        ConfigureMongoDb();
    }

    private static void ConfigureMongoDb()
    {
        // Passo 1º: Configurar o tipo do serializador de Guid.
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));

        // Passo 2º: Configurar as convenções, assim será aplicado para todos os mapeamentos.
        // REF: https://mongodb.github.io/mongo-csharp-driver/2.0/reference/bson/mapping/conventions/
        ConventionRegistry.Register("Conventions",
            new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String),
                new IgnoreExtraElementsConvention(true),
                new IgnoreIfNullConvention(true)
            }, _ => true);

        // Passo 3º: Registrar as configurações dos mapeamento das classes.
        // É recomendável registrar todos os mapeamentos antes de inicializar a conexão com o MongoDb
        // REF: https://mongodb.github.io/mongo-csharp-driver/2.0/reference/bson/mapping/
        new BaseQueryModelMap().Configure();
        new CustomerMap().Configure();
    }
}