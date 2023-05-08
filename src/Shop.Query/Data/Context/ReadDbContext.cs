using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Shop.Query.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Context;

public sealed class ReadDbContext : IReadDbContext
{
    private const string DatabaseName = "Shop";
    private static readonly Random Rnd = new();

    private static readonly ReplaceOptions DefaultReplaceOptions = new()
    {
        // Indica se o documento deve ser inserido se ele não existir.
        IsUpsert = true
    };

    private static readonly CreateIndexOptions DefaultCreateIndexOptions = new()
    {
        // Restrição Exclusiva
        Unique = true,

        // Índices esparsos são como índices não esparsos, exceto que eles omitem referências a documentos que não incluem o campo indexado.
        // Para campos que estão presentes apenas em alguns documentos, índices esparsos podem fornecer uma economia significativa de espaço.
        Sparse = true
    };

    private readonly IMongoDatabase _database;
    private readonly ILogger<ReadDbContext> _logger;
    private readonly AsyncRetryPolicy _mongoRetryPolicy;

    public ReadDbContext(IOptions<ConnectionOptions> options, ILogger<ReadDbContext> logger)
    {
        ConnectionString = options.Value.NoSqlConnection;

        var mongoClient = new MongoClient(options.Value.NoSqlConnection);
        _database = mongoClient.GetDatabase(DatabaseName);
        _logger = logger;
        _mongoRetryPolicy = CreateRetryPolicy(logger);
    }

    public string ConnectionString { get; }

    public IMongoCollection<TQueryModel> GetCollection<TQueryModel>() where TQueryModel : IQueryModel
        => _database.GetCollection<TQueryModel>(typeof(TQueryModel).Name);

    public async Task UpsertAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter)
        where TQueryModel : IQueryModel
    {
        var collection = GetCollection<TQueryModel>();

        await _mongoRetryPolicy
            .ExecuteAsync(async () =>
                await collection.ReplaceOneAsync(upsertFilter, queryModel, DefaultReplaceOptions));
    }

    public async Task DeleteAsync<TQueryModel>(Expression<Func<TQueryModel, bool>> deleteFilter)
        where TQueryModel : IQueryModel
    {
        var collection = GetCollection<TQueryModel>();
        await _mongoRetryPolicy
            .ExecuteAsync(async () => await collection.DeleteOneAsync(deleteFilter));
    }

    public async Task CreateCollectionsAsync()
    {
        // Obtendo as coleções existentes da base.
        using var asyncCursor = await _database.ListCollectionNamesAsync();
        var collections = await asyncCursor.ToListAsync();

        foreach (var collectionName in GetCollectionNamesFromAssembly())
        {
            if (!collections.Any(name => string.Equals(name, collectionName, StringComparison.Ordinal)))
            {
                _logger.LogInformation("----- MongoDB: criando a coleção {Name}", collectionName);
                await _database.CreateCollectionAsync(collectionName);
            }
            else
            {
                _logger.LogInformation("----- MongoDB: a coleção {Name} já existe", collectionName);
            }
        }

        await CreateIndexAsync();
    }

    private async Task CreateIndexAsync()
    {
        var indexDefinition = Builders<CustomerQueryModel>.IndexKeys.Ascending(model => model.Email);
        var indexModel = new CreateIndexModel<CustomerQueryModel>(indexDefinition, DefaultCreateIndexOptions);
        var collection = GetCollection<CustomerQueryModel>();
        await collection.Indexes.CreateOneAsync(indexModel);
    }

    private static IEnumerable<string> GetCollectionNamesFromAssembly()
        => Assembly
            .GetExecutingAssembly()
            .GetAllTypesOf<IQueryModel>()
            .Select(impl => impl.Name)
            .ToList();

    private static AsyncRetryPolicy CreateRetryPolicy(ILogger logger)
    {
        TimeSpan SleepDurationProvider(int retryAttempt)
        {
            // Retry with jitter
            // A well-known retry strategy is exponential backoff, allowing retries to be made initially quickly,
            // but then at progressively longer intervals: for example, after 2, 4, 8, 15, then 30 seconds.
            // REF: https://github.com/App-vNext/Polly/wiki/Retry-with-jitter#simple-jitter
            var sleepDuration = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                                TimeSpan.FromMilliseconds(Rnd.Next(0, 1000));

            logger.LogWarning("----- MongoDB: Retry #{Count} with delay {Delay}", retryAttempt, sleepDuration);

            return sleepDuration;
        }

        void OnRetry(Exception ex, TimeSpan _) => logger.LogError(ex,
            "Ocorreu uma exceção não esperada ao salvar no MongoDB: {Message}", ex.Message);

        return Policy.Handle<MongoException>().WaitAndRetryAsync(2, SleepDurationProvider, OnRetry);
    }
}