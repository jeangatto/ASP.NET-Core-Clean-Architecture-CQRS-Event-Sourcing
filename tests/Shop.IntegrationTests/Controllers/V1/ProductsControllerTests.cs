using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shop.Application.Product.Commands;
using Shop.Application.Product.Responses;
using Shop.Core.Extensions;
using Shop.Domain.Entities.ProductAggregate;
using Shop.Infrastructure.Data.Context;
using Shop.IntegrationTests.Extensions;
using Shop.PublicApi.Models;
using Shop.Query.Abstractions;
using Shop.Query.Data.Context;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;
using Xunit;
using Xunit.Categories;

namespace Shop.IntegrationTests.Controllers.V1;

[IntegrationTest]
public class ProductsControllerTests : IAsyncLifetime
{
    private const string ConnectionString = "Data Source=:memory:";
    private const string Endpoint = "/api/products";
    private readonly SqliteConnection _eventStoreDbContextSqlite = new(ConnectionString);
    private readonly SqliteConnection _writeDbContextSqlite = new(ConnectionString);

    #region POST: /api/products/

    [Fact]
    public async Task Should_ReturnsHttpStatus200Ok_When_Post_ValidRequest()
    {
        // Arrange
        await using var webApplicationFactory = InitializeWebAppFactory();
        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        var command = new Faker<CreateProductCommand>()
            .RuleFor(command => command.Name, faker => faker.Commerce.ProductName())
            .RuleFor(command => command.Description, faker => faker.Commerce.ProductDescription())
            .RuleFor(command => command.Price, faker => faker.Random.Decimal(1, 1000))
            .Generate();

        var commandAsJsonString = command.ToJson();

        // Act
        using var jsonContent = new StringContent(commandAsJsonString, Encoding.UTF8, MediaTypeNames.Application.Json);
        using var act = await httpClient.PostAsync(Endpoint, jsonContent);

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeTrue();
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<CreatedProductResponse>>();
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Errors.Should().BeEmpty();
        response.Result.Should().NotBeNull();
        response.Result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnsHttpStatus400BadRequest_When_Post_InvalidRequest()
    {
        // Arrange
        await using var webApplicationFactory = InitializeWebAppFactory();
        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        var command = new Faker<CreateProductCommand>().Generate();
        var commandAsJsonString = command.ToJson();

        // Act
        using var jsonContent = new StringContent(commandAsJsonString, Encoding.UTF8, MediaTypeNames.Application.Json);
        using var act = await httpClient.PostAsync(Endpoint, jsonContent);

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<CreatedProductResponse>>();
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        response.Result.Should().BeNull();
        response.Errors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
    }

    #endregion

    #region GET: /api/products/

    [Fact]
    public async Task Should_ReturnsHttpStatus200Ok_When_GetAll()
    {
        // Arrange
        var queryModels = new Faker<ProductQueryModel>()
            .UsePrivateConstructor()
            .RuleFor(queryModel => queryModel.Id, faker => faker.Random.Guid())
            .RuleFor(queryModel => queryModel.Name, faker => faker.Commerce.ProductName())
            .RuleFor(queryModel => queryModel.Description, faker => faker.Commerce.ProductDescription())
            .RuleFor(queryModel => queryModel.Price, faker => faker.Random.Decimal(1, 1000))
            .Generate(10);

        var readOnlyRepository = Substitute.For<IProductReadOnlyRepository>();
        readOnlyRepository.GetAllAsync().Returns(queryModels);

        await using var webApplicationFactory = InitializeWebAppFactory(services =>
        {
            services.RemoveAll<IProductReadOnlyRepository>();
            services.AddScoped(_ => readOnlyRepository);
        });

        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        // Act
        using var act = await httpClient.GetAsync(Endpoint);

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeTrue();
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<IEnumerable<ProductQueryModel>>>();
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Errors.Should().BeEmpty();
        response.Result.Should().NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.HaveCount(queryModels.Count)
            .And.AllSatisfy(model =>
            {
                model.Id.Should().NotBeEmpty();
                model.Name.Should().NotBeNullOrWhiteSpace();
                model.Description.Should().NotBeNullOrWhiteSpace();
                model.Price.Should().BePositive();
            });

        await readOnlyRepository.Received(1).GetAllAsync();
    }

    #endregion

    #region GET: /api/products/{id}

    [Fact]
    public async Task Should_ReturnsHttpStatus200Ok_When_GetById_ValidRequest()
    {
        // Arrange
        var queryModel = new Faker<ProductQueryModel>()
            .UsePrivateConstructor()
            .RuleFor(queryModel => queryModel.Id, faker => faker.Random.Guid())
            .RuleFor(queryModel => queryModel.Name, faker => faker.Commerce.ProductName())
            .RuleFor(queryModel => queryModel.Description, faker => faker.Commerce.ProductDescription())
            .RuleFor(queryModel => queryModel.Price, faker => faker.Random.Decimal(1, 1000))
            .Generate();

        var readOnlyRepository = Substitute.For<IProductReadOnlyRepository>();
        readOnlyRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(queryModel);

        await using var webApplicationFactory = InitializeWebAppFactory(services =>
        {
            services.RemoveAll<IProductReadOnlyRepository>();
            services.AddScoped(_ => readOnlyRepository);
        });

        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        // Act
        using var act = await httpClient.GetAsync($"{Endpoint}/{queryModel.Id}");

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeTrue();
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<ProductQueryModel>>();
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Errors.Should().BeEmpty();
        response.Result.Should().NotBeNull();
        response.Result.Id.Should().NotBeEmpty().And.Be(queryModel.Id);
        response.Result.Name.Should().NotBeNullOrWhiteSpace().And.Be(queryModel.Name);
        response.Result.Description.Should().NotBeNullOrWhiteSpace().And.Be(queryModel.Description);
        response.Result.Price.Should().Be(queryModel.Price);

        await readOnlyRepository.Received(1).GetByIdAsync(Arg.Is<Guid>(id => id == queryModel.Id));
    }

    [Fact]
    public async Task Should_ReturnsHttpStatus400BadRequest_When_GetById_InvalidRequest()
    {
        // Arrange
        var readOnlyRepository = Substitute.For<IProductReadOnlyRepository>();
        readOnlyRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((ProductQueryModel)null);

        await using var webApplicationFactory = InitializeWebAppFactory(services =>
        {
            services.RemoveAll<IProductReadOnlyRepository>();
            services.AddScoped(_ => readOnlyRepository);
        });

        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        var productId = Guid.Empty;

        // Act
        using var act = await httpClient.GetAsync($"{Endpoint}/{productId}");

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<ProductQueryModel>>();
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        response.Result.Should().BeNull();
        response.Errors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();

        await readOnlyRepository.DidNotReceive().GetByIdAsync(Arg.Is<Guid>(id => id == productId));
    }

    [Fact]
    public async Task Should_ReturnsStatus404NotFound_When_GetById_NonExistingProduct()
    {
        // Arrange
        var readOnlyRepository = Substitute.For<IProductReadOnlyRepository>();
        readOnlyRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((ProductQueryModel)null);

        await using var webApplicationFactory = InitializeWebAppFactory(services =>
        {
            services.RemoveAll<IProductReadOnlyRepository>();
            services.AddScoped(_ => readOnlyRepository);
        });

        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        var productId = Guid.NewGuid();

        // Act
        using var act = await httpClient.GetAsync($"{Endpoint}/{productId}");

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<ProductQueryModel>>();
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        response.Result.Should().BeNull();
        response.Errors.Should().NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.AllSatisfy(error => error.Message.Should().Be($"No Product found by Id: {productId}"));

        await readOnlyRepository.Received(1).GetByIdAsync(Arg.Is<Guid>(id => id == productId));
    }

    #endregion

    #region DELETE: /api/products/{id}

    [Fact]
    public async Task Should_ReturnsHttpStatus200Ok_When_Delete_ValidRequest()
    {
        // Arrange
        var product = new Faker<Product>()
            .RuleFor(p => p.Name, faker => faker.Commerce.ProductName())
            .RuleFor(p => p.Description, faker => faker.Commerce.ProductDescription())
            .RuleFor(p => p.Price, faker => faker.Random.Decimal(1, 1000))
            .Generate();

        await using var webApplicationFactory = InitializeWebAppFactory(configureServiceScope: serviceScope =>
        {
            var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
            writeDbContext.Products.Add(product);
            writeDbContext.SaveChanges();
        });

        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        // Act
        using var act = await httpClient.DeleteAsync($"{Endpoint}/{product.Id}");

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeTrue();
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse>();
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_ReturnsHttpStatus400BadRequest_When_Delete_InvalidRequest()
    {
        // Arrange
        await using var webApplicationFactory = InitializeWebAppFactory();
        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        // Act
        using var act = await httpClient.DeleteAsync($"{Endpoint}/{Guid.Empty}");

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse>();
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        response.Errors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task Should_ReturnsStatus404NotFound_When_Delete_NonExistingProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();

        await using var webApplicationFactory = InitializeWebAppFactory();
        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        // Act
        using var act = await httpClient.DeleteAsync($"{Endpoint}/{productId}");

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse>();
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        response.Errors.Should().NotBeNullOrEmpty()
        .And.OnlyHaveUniqueItems()
        .And.AllSatisfy(error => error.Message.Should().Be($"No Product found by Id: {productId}"));
    }
    #endregion

    #region IAsyncLifetime

    public async Task InitializeAsync()
    {
        await _writeDbContextSqlite.OpenAsync();
        await _eventStoreDbContextSqlite.OpenAsync();
    }

    public async Task DisposeAsync()
    {
        await _writeDbContextSqlite.DisposeAsync();
        await _eventStoreDbContextSqlite.DisposeAsync();
    }

    #endregion

    #region Helpers

    private WebApplicationFactory<Program> InitializeWebAppFactory(
        Action<IServiceCollection> configureServices = null,
        Action<IServiceScope> configureServiceScope = null)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(hostBuilder =>
            {
                hostBuilder.UseSetting("ConnectionStrings:SqlConnection", "InMemory");
                hostBuilder.UseSetting("ConnectionStrings:NoSqlConnection", "InMemory");
                hostBuilder.UseSetting("ConnectionStrings:CacheConnection", "InMemory");

                hostBuilder.UseSetting("CacheOptions:AbsoluteExpirationInHours", "1");
                hostBuilder.UseSetting("CacheOptions:SlidingExpirationInSeconds", "30");

                hostBuilder.UseEnvironment(Environments.Production);

                hostBuilder.ConfigureLogging(logging => logging.ClearProviders());

                hostBuilder.ConfigureServices(services =>
                {
                    services.RemoveAll<WriteDbContext>();
                    services.RemoveAll<DbContextOptions<WriteDbContext>>();
                    services.RemoveAll<EventStoreDbContext>();
                    services.RemoveAll<DbContextOptions<EventStoreDbContext>>();
                    services.RemoveAll<NoSqlDbContext>();
                    services.RemoveAll<ISynchronizeDb>();

                    services.AddDbContext<WriteDbContext>(
                        options => options.UseSqlite(_writeDbContextSqlite));

                    services.AddDbContext<EventStoreDbContext>(
                        options => options.UseSqlite(_eventStoreDbContextSqlite));

                    services.AddSingleton(_ => Substitute.For<IReadDbContext>());
                    services.AddSingleton(_ => Substitute.For<ISynchronizeDb>());

                    configureServices?.Invoke(services);

                    using var serviceProvider = services.BuildServiceProvider(true);
                    using var serviceScope = serviceProvider.CreateScope();

                    var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
                    writeDbContext.Database.EnsureCreated();

                    var eventStoreDbContext = serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
                    eventStoreDbContext.Database.EnsureCreated();

                    configureServiceScope?.Invoke(serviceScope);

                    writeDbContext.Dispose();
                    eventStoreDbContext.Dispose();
                });
            });
    }

    private static WebApplicationFactoryClientOptions CreateClientOptions() => new() { AllowAutoRedirect = false };

    #endregion
}