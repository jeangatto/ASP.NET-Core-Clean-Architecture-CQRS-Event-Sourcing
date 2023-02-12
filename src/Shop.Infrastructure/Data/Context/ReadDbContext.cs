using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shop.Core.AppSettings;
using Shop.Core.Events;
using Shop.Core.Interfaces;

namespace Shop.Infrastructure.Data.Context;

public class ReadDbContext
{
    private const string DatabaseName = "Events";

    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly ILogger<ReadDbContext> _logger;
    private readonly string _connectionString;

    public ReadDbContext(IOptions<ConnectionOptions> options, ILogger<ReadDbContext> logger)
    {
        _connectionString = options.Value.EventConnection;

        _client = new MongoClient(_connectionString);
        _database = _client.GetDatabase(DatabaseName);
        _logger = logger;
    }

    public IMongoCollection<T> GetCollection<T>() => _database.GetCollection<T>(typeof(T).Name);

    public string GetConnectionString() => _connectionString;

    public async Task CreateCollectionsAsync()
    {
        // Obtendo as coleções existentes da base.
        using var asynCursor = await _database.ListCollectionNamesAsync();
        var collections = await asynCursor.ToListAsync();

        var collectionNames = new List<string> { nameof(EventStore) };
        collectionNames.AddRange(GetCollectionNamesFromAssembly());

        foreach (var collectionName in collectionNames)
        {
            if (!collections.Any(name => name == collectionName))
            {
                _logger.LogInformation("----- MongoDB: criando a coleção {Name}", collectionName);
                await _database.CreateCollectionAsync(collectionName);
            }
            else
            {
                _logger.LogInformation("----- MongoDB: a coleção {Name} já existe", collectionName);
            }
        }
    }

    private static IEnumerable<string> GetCollectionNamesFromAssembly()
    {
        var collectionNames = new List<string>();

        var implementations = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IQueryModel).IsAssignableFrom(type)
                && type.IsClass
                && !type.IsAbstract
                && !type.IsInterface)
            .ToList();

        foreach (var impl in implementations)
        {
            collectionNames.Add(impl.Name);
        }

        return collectionNames;
    }
}