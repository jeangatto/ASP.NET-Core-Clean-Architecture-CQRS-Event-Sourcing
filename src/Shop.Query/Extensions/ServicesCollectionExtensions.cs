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
        services
            .AddMediatR(executingAssembly)
            .AddSingleton<IMapper>(new Mapper(new MapperConfiguration(cfg => cfg.AddMaps(executingAssembly))))
            .AddValidatorsFromAssembly(executingAssembly).AddSingleton<IReadDbContext, ReadDbContext>()
            .AddScoped<ICustomerReadOnlyRepository, CustomerReadOnlyRepository>();

        ConfigureMongoDb();
    }

    private static void ConfigureMongoDb()
    {
        // Step 1: Configure the serializer for Guid type.
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));

        // Step 2: Configure the conventions to be applied to all mappings.
        // REF: https://mongodb.github.io/mongo-csharp-driver/2.0/reference/bson/mapping/conventions/
        ConventionRegistry.Register("Conventions",
            new ConventionPack
            {
                new CamelCaseElementNameConvention(), // Convert element names to camel case
                new EnumRepresentationConvention(BsonType.String), // Serialize enums as strings
                new IgnoreExtraElementsConvention(true), // Ignore extra elements when deserializing
                new IgnoreIfNullConvention(true) // Ignore null values when serializing
            }, _ => true);

        // Step 3: Register the mappings configurations.
        // It is recommended to register all mappings before initializing the connection with MongoDb
        // REF: https://mongodb.github.io/mongo-csharp-driver/2.0/reference/bson/mapping/
        new BaseQueryModelMap().Configure(); // Configuration for base abstract class
        new CustomerMap().Configure(); // Configuration for Customer class
    }
}