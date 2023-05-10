using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Shop.Application.Customer.Commands;
using Shop.Application.Customer.Handlers;
using Shop.Core.Abstractions;
using Shop.Core.Events;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.Factories;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Data.Repositories;
using Shop.UnitTests.Fixtures;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Application.Customer.Handlers;

[UnitTest]
public class UpdateCustomerCommandHandlerTest : IClassFixture<EfSqliteFixture>
{
    private readonly EfSqliteFixture _fixture;
    private readonly UpdateCustomerCommandValidator _validator = new();

    public UpdateCustomerCommandHandlerTest(EfSqliteFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Update_ValidCommand_ShouldReturnsSuccessResult()
    {
        // Arrange
        var customer = new Faker<Shop.Domain.Entities.CustomerAggregate.Customer>()
            .CustomInstantiator(faker => CustomerFactory.Create(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.PickRandom<EGender>(),
                faker.Person.Email,
                faker.Person.DateOfBirth))
            .Generate();

        var repository = new CustomerWriteOnlyRepository(_fixture.Context);
        repository.Add(customer);

        await _fixture.Context.SaveChangesAsync();
        _fixture.Context.ChangeTracker.Clear();

        var unitOfWork = new UnitOfWork(
            _fixture.Context,
            Mock.Of<IEventStoreRepository>(),
            Mock.Of<IMediator>(),
            Mock.Of<ILogger<UnitOfWork>>());

        var command = new Faker<UpdateCustomerCommand>()
            .RuleFor(command => command.Id, customer.Id)
            .RuleFor(command => command.Email, faker => faker.Person.Email.ToLowerInvariant())
            .Generate();

        var handler = new UpdateCustomerCommandHandler(_validator, repository, unitOfWork);

        // Act
        var act = await handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.SuccessMessage.Should().Be("Atualizado com sucesso!");
    }

    [Fact]
    public async Task Update_DuplicateEmailCommand_ShouldReturnsFailResult()
    {
        // Arrange
        var customers = new Faker<Shop.Domain.Entities.CustomerAggregate.Customer>()
            .CustomInstantiator(faker => CustomerFactory.Create(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.PickRandom<EGender>(),
                faker.Person.Email,
                faker.Person.DateOfBirth))
            .Generate(2);

        var repository = new CustomerWriteOnlyRepository(_fixture.Context);

        foreach (var customer in customers)
        {
            repository.Add(customer);
        }

        await _fixture.Context.SaveChangesAsync();
        _fixture.Context.ChangeTracker.Clear();

        var command = new Faker<UpdateCustomerCommand>()
            .RuleFor(command => command.Id, customers[0].Id) // O ID do primeiro customer
            .RuleFor(command => command.Email, _ => customers[1].Email.Address) // O E-mail do segundo customer
            .Generate();

        var handler = new UpdateCustomerCommandHandler(_validator, repository, Mock.Of<IUnitOfWork>());

        // Act
        var act = await handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Errors.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.Contain(errorMessage => errorMessage == "O endereço de e-mail informado já está sendo utilizado.");
    }

    [Fact]
    public async Task Update_NotFoundCustomer_ShouldReturnsFailResult()
    {
        // Arrange
        var command = new Faker<UpdateCustomerCommand>()
            .RuleFor(command => command.Id, faker => faker.Random.Guid())
            .RuleFor(command => command.Email, faker => faker.Person.Email)
            .Generate();

        var handler = new UpdateCustomerCommandHandler(
            _validator,
            new CustomerWriteOnlyRepository(_fixture.Context),
            Mock.Of<IUnitOfWork>());

        // Act
        var act = await handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Errors.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.Contain(errorMessage => errorMessage == $"Nenhum cliente encontrado pelo Id: {command.Id}");
    }

    [Fact]
    public async Task Update_InvalidCommand_ShouldReturnsFailResult()
    {
        // Arrange
        var handler = new UpdateCustomerCommandHandler(
            _validator,
            Mock.Of<ICustomerWriteOnlyRepository>(),
            Mock.Of<IUnitOfWork>());

        // Act
        var act = await handler.Handle(new UpdateCustomerCommand(), CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.ValidationErrors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
    }
}