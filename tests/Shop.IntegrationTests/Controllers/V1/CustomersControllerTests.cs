using System;
using System.Collections.Generic;
using System.Net;
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
using Shop.Application.Customer.Commands;
using Shop.Application.Customer.Responses;
using Shop.Core.Extensions;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.ValueObjects;
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
public class CustomersControllerTests : IAsyncLifetime
{
    private const string ConnectionString = "Data Source=:memory:";
    private const string Endpoint = "/api/customers";
    private readonly SqliteConnection _eventStoreDbContextSqlite = new(ConnectionString);
    private readonly SqliteConnection _writeDbContextSqlite = new(ConnectionString);

    #region POST: /api/customer/

    [Fact]
    public async Task Should_ReturnsHttpStatus201Created_When_Post_ValidRequest()
    {
        // Arrange
        await using var webApplicationFactory = InitializeWebAppFactory();
        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        var command = new Faker<CreateCustomerCommand>()
            .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
            .RuleFor(command => command.LastName, faker => faker.Person.LastName)
            .RuleFor(command => command.Email, faker => faker.Person.Email)
            .RuleFor(command => command.Gender, faker => faker.PickRandom<EGender>())
            .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
            .Generate();

        // Act
        using var jsonContent = command.ToJsonHttpContent();
        using var act = await httpClient.PostAsync(Endpoint, jsonContent);

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeTrue();
        act.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<CreatedCustomerResponse>>();
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(StatusCodes.Status201Created);
        response.Errors.Should().BeEmpty();
        response.Result.Should().NotBeNull();
        response.Result.Id.Should().NotBeEmpty();

        // Assert Location Header
        act.Headers.GetValues("Location").Should().NotBeNullOrEmpty()
            .And.Contain($"/api/customers/{response.Result.Id}");
    }

    [Fact]
    public async Task Should_ReturnsHttpStatus400BadRequest_When_Post_InvalidRequest()
    {
        // Arrange
        await using var webApplicationFactory = InitializeWebAppFactory();
        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        var command = new Faker<CreateCustomerCommand>().Generate();

        // Act
        using var jsonContent = command.ToJsonHttpContent();
        using var act = await httpClient.PostAsync(Endpoint, jsonContent);

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<CreatedCustomerResponse>>();
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        response.Result.Should().BeNull();
        response.Errors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task Should_ReturnsHttpStatus400BadRequest_When_Post_EmailAddressIsAlready()
    {
        // Arrange
        var customer = new Faker<Customer>()
            .CustomInstantiator(faker =>
            {
                var emailResult = Email.Create(faker.Person.Email);
                return new Customer(
                    faker.Person.FirstName,
                    faker.Person.LastName,
                    faker.PickRandom<EGender>(),
                    emailResult.Value,
                    faker.Person.DateOfBirth);
            })
            .Generate();

        await using var webApplicationFactory = InitializeWebAppFactory(configureServiceScope: serviceScope =>
        {
            var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
            writeDbContext.Customers.Add(customer);
            writeDbContext.SaveChanges();
        });
        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        var command = new Faker<CreateCustomerCommand>()
            .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
            .RuleFor(command => command.LastName, faker => faker.Person.LastName)
            .RuleFor(command => command.Email, customer.Email.Address)
            .RuleFor(command => command.Gender, faker => faker.PickRandom<EGender>())
            .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
            .Generate();

        // Act
        using var jsonContent = command.ToJsonHttpContent();
        using var act = await httpClient.PostAsync(Endpoint, jsonContent);

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<CreatedCustomerResponse>>();
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        response.Result.Should().BeNull();
        response.Errors.Should().NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.AllSatisfy(error => error.Message.Should().Be("The provided email address is already in use."));
    }

    #endregion

    #region GET: /api/customer/

    [Fact]
    public async Task Should_ReturnsHttpStatus200Ok_When_GetAll()
    {
        // Arrange
        var queryModels = new Faker<CustomerQueryModel>()
            .UsePrivateConstructor()
            .RuleFor(queryModel => queryModel.Id, faker => faker.Random.Guid())
            .RuleFor(queryModel => queryModel.FirstName, faker => faker.Person.FirstName)
            .RuleFor(queryModel => queryModel.LastName, faker => faker.Person.LastName)
            .RuleFor(queryModel => queryModel.Email, faker => faker.Person.Email)
            .RuleFor(queryModel => queryModel.Gender, faker => faker.PickRandom<EGender>().ToString())
            .RuleFor(queryModel => queryModel.DateOfBirth, faker => faker.Person.DateOfBirth)
            .Generate(10);

        var readOnlyRepository = Substitute.For<ICustomerReadOnlyRepository>();
        readOnlyRepository.GetAllAsync().Returns(queryModels);

        await using var webApplicationFactory = InitializeWebAppFactory(services =>
        {
            services.RemoveAll<ICustomerReadOnlyRepository>();
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
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<IEnumerable<CustomerQueryModel>>>();
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
                model.FirstName.Should().NotBeNullOrWhiteSpace();
                model.LastName.Should().NotBeNullOrWhiteSpace();
                model.Email.Should().NotBeNullOrWhiteSpace();
                model.Gender.Should().NotBeNullOrWhiteSpace();
                model.FullName.Should().NotBeNullOrWhiteSpace();
            });

        await readOnlyRepository.Received(1).GetAllAsync();
    }

    #endregion

    #region GET: /api/customer/{id}

    [Fact]
    public async Task Should_ReturnsHttpStatus200Ok_When_GetById_ValidRequest()
    {
        // Arrange
        var queryModel = new Faker<CustomerQueryModel>()
            .UsePrivateConstructor()
            .RuleFor(queryModel => queryModel.Id, faker => faker.Random.Guid())
            .RuleFor(queryModel => queryModel.FirstName, faker => faker.Person.FirstName)
            .RuleFor(queryModel => queryModel.LastName, faker => faker.Person.LastName)
            .RuleFor(queryModel => queryModel.Email, faker => faker.Person.Email)
            .RuleFor(queryModel => queryModel.Gender, faker => faker.PickRandom<EGender>().ToString())
            .RuleFor(queryModel => queryModel.DateOfBirth, faker => faker.Person.DateOfBirth)
            .Generate();

        var readOnlyRepository = Substitute.For<ICustomerReadOnlyRepository>();
        readOnlyRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(queryModel);

        await using var webApplicationFactory = InitializeWebAppFactory(services =>
        {
            services.RemoveAll<ICustomerReadOnlyRepository>();
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
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<CustomerQueryModel>>();
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Errors.Should().BeEmpty();
        response.Result.Should().NotBeNull();
        response.Result.Id.Should().NotBeEmpty().And.Be(queryModel.Id);
        response.Result.FirstName.Should().NotBeNullOrWhiteSpace().And.Be(queryModel.FirstName);
        response.Result.LastName.Should().NotBeNullOrWhiteSpace().And.Be(queryModel.LastName);
        response.Result.Email.Should().NotBeNullOrWhiteSpace().And.Be(queryModel.Email);
        response.Result.Gender.Should().NotBeNullOrWhiteSpace().And.Be(queryModel.Gender);
        response.Result.DateOfBirth.Should().Be(queryModel.DateOfBirth);
        response.Result.FullName.Should().NotBeNullOrWhiteSpace().And.Be(queryModel.FullName);

        await readOnlyRepository.Received(1).GetByIdAsync(Arg.Is<Guid>(id => id == queryModel.Id));
    }

    [Fact]
    public async Task Should_ReturnsHttpStatus400BadRequest_When_GetById_InvalidRequest()
    {
        // Arrange
        var readOnlyRepository = Substitute.For<ICustomerReadOnlyRepository>();
        readOnlyRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((CustomerQueryModel)null);

        await using var webApplicationFactory = InitializeWebAppFactory(services =>
        {
            services.RemoveAll<ICustomerReadOnlyRepository>();
            services.AddScoped(_ => readOnlyRepository);
        });

        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        var customerId = Guid.Empty;

        // Act
        using var act = await httpClient.GetAsync($"{Endpoint}/{customerId}");

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<CustomerQueryModel>>();
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        response.Result.Should().BeNull();
        response.Errors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();

        await readOnlyRepository.DidNotReceive().GetByIdAsync(Arg.Is<Guid>(id => id == customerId));
    }

    [Fact]
    public async Task Should_ReturnsStatus404NotFound_When_GetById_NonExistingCustomer()
    {
        // Arrange
        var readOnlyRepository = Substitute.For<ICustomerReadOnlyRepository>();
        readOnlyRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((CustomerQueryModel)null);

        await using var webApplicationFactory = InitializeWebAppFactory(services =>
        {
            services.RemoveAll<ICustomerReadOnlyRepository>();
            services.AddScoped(_ => readOnlyRepository);
        });

        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        var customerId = Guid.NewGuid();

        // Act
        using var act = await httpClient.GetAsync($"{Endpoint}/{customerId}");

        // Assert (HTTP)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Assert (HTTP Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<CustomerQueryModel>>();
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        response.Result.Should().BeNull();
        response.Errors.Should().NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.AllSatisfy(error => error.Message.Should().Be($"No customer found by Id: {customerId}"));

        await readOnlyRepository.Received(1).GetByIdAsync(Arg.Is<Guid>(id => id == customerId));
    }

    #endregion

    #region DELETE: /api/customer/{id}

    [Fact]
    public async Task Should_ReturnsHttpStatus200Ok_When_Delete_ValidRequest()
    {
        // Arrange
        var customer = new Faker<Customer>()
            .CustomInstantiator(faker =>
            {
                var emailResult = Email.Create(faker.Person.Email);
                return new Customer(
                    faker.Person.FirstName,
                    faker.Person.LastName,
                    faker.PickRandom<EGender>(),
                    emailResult.Value,
                    faker.Person.DateOfBirth);
            })
            .Generate();

        await using var webApplicationFactory = InitializeWebAppFactory(configureServiceScope: serviceScope =>
        {
            var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
            writeDbContext.Customers.Add(customer);
            writeDbContext.SaveChanges();
        });

        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        // Act
        using var act = await httpClient.DeleteAsync($"{Endpoint}/{customer.Id}");

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
    public async Task Should_ReturnsStatus404NotFound_When_Delete_NonExistingCustomer()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        await using var webApplicationFactory = InitializeWebAppFactory();
        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        // Act
        using var act = await httpClient.DeleteAsync($"{Endpoint}/{customerId}");

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
            .And.AllSatisfy(error => error.Message.Should().Be($"No customer found by Id: {customerId}"));
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