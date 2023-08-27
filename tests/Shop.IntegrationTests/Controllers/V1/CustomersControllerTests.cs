using System;
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
using NSubstitute;
using Shop.Application.Customer.Commands;
using Shop.Application.Customer.Responses;
using Shop.Core.Extensions;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.ValueObjects;
using Shop.Infrastructure.Data.Context;
using Shop.PublicApi.Models;
using Shop.Query.Abstractions;
using Shop.Query.Data.Context;
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

    [Fact]
    public async Task Should_ReturnsHttpStatus200Ok_When_Post_ValidRequest()
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

        var commandAsJsonString = command.ToJson();

        // Act
        using var jsonContent = new StringContent(commandAsJsonString, Encoding.UTF8, MediaTypeNames.Application.Json);
        using var act = await httpClient.PostAsync(Endpoint, jsonContent);

        // Assert (Http)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeTrue();
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert (Http Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<CreatedCustomerResponse>>();
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Result.Should().NotBeNull();
        response.Result.Id.Should().NotBeEmpty();
        response.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_ReturnsHttpStatus400BadRequest_When_Post_InvalidRequest()
    {
        // Arrange
        await using var webApplicationFactory = InitializeWebAppFactory();
        using var httpClient = webApplicationFactory.CreateClient(CreateClientOptions());

        var command = new Faker<CreateCustomerCommand>().Generate();
        var commandAsJsonString = command.ToJson();

        // Act
        using var jsonContent = new StringContent(commandAsJsonString, Encoding.UTF8, MediaTypeNames.Application.Json);
        using var act = await httpClient.PostAsync(Endpoint, jsonContent);

        // Assert (Http)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert (Http Content Response)
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

        await using var webApplicationFactory = InitializeWebAppFactory((serviceScope) =>
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

        var commandAsJsonString = command.ToJson();

        // Act
        using var jsonContent = new StringContent(commandAsJsonString, Encoding.UTF8, MediaTypeNames.Application.Json);
        using var act = await httpClient.PostAsync(Endpoint, jsonContent);

        // Assert (Http)
        act.Should().NotBeNull();
        act.IsSuccessStatusCode.Should().BeFalse();
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert (Http Content Response)
        var response = (await act.Content.ReadAsStringAsync()).FromJson<ApiResponse<CreatedCustomerResponse>>();
        response.Should().NotBeNull();
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        response.Result.Should().BeNull();
        response.Errors.Should().NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.AllSatisfy(error => error.Message.Should().Be("The provided email address is already in use."));
    }

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

    private WebApplicationFactory<Program> InitializeWebAppFactory(Action<IServiceScope> configureServices = null)
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

                hostBuilder.ConfigureServices(services =>
                {
                    services.RemoveAll<WriteDbContext>();
                    services.RemoveAll<DbContextOptions<WriteDbContext>>();
                    services.RemoveAll<EventStoreDbContext>();
                    services.RemoveAll<DbContextOptions<EventStoreDbContext>>();
                    services.RemoveAll<NoSqlDbContext>();
                    services.RemoveAll<ISynchronizeDb>();

                    services.AddDbContext<WriteDbContext>(options => options.UseSqlite(_writeDbContextSqlite));
                    services.AddDbContext<EventStoreDbContext>(options => options.UseSqlite(_eventStoreDbContextSqlite));
                    services.AddSingleton(_ => Substitute.For<IReadDbContext>());
                    services.AddSingleton(_ => Substitute.For<ISynchronizeDb>());

                    var serviceProvider = services.BuildServiceProvider(true);
                    using var serviceScope = serviceProvider.CreateScope();

                    var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
                    writeDbContext.Database.EnsureCreated();

                    var eventStoreDbContext = serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
                    eventStoreDbContext.Database.EnsureCreated();

                    configureServices?.Invoke(serviceScope);

                    writeDbContext.Dispose();
                    eventStoreDbContext.Dispose();
                });
            });
    }

    private static WebApplicationFactoryClientOptions CreateClientOptions() =>
        new() { BaseAddress = new Uri("https://127.0.0.1"), AllowAutoRedirect = false };
}