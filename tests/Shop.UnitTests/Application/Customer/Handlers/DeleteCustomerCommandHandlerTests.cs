using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Shop.Application.Customer.Commands;
using Shop.Application.Customer.Handlers;
using Shop.Core.Shared;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.Factories;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Data.Repositories;
using Shop.UnitTests.Fixtures;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Application.Customer.Handlers;

[UnitTest]
public class DeleteCustomerCommandHandlerTests : IClassFixture<EfSqliteFixture>
{
    private readonly EfSqliteFixture _fixture;
    private readonly DeleteCustomerCommandValidator _validator = new();

    public DeleteCustomerCommandHandlerTests(EfSqliteFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Delete_ValidCustomerId_ShouldReturnsSuccessResult()
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

        var handler = new DeleteCustomerCommandHandler(
            _validator,
            new CustomerWriteOnlyRepository(_fixture.Context),
            unitOfWork);

        var command = new DeleteCustomerCommand(customer.Id);

        // Act
        var act = await handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.SuccessMessage.Should().Be("Removido com sucesso!");
    }

    [Fact]
    public async Task Delete_NotFoundCustomer_ShouldReturnsFailResult()
    {
        // Arrange
        var command = new DeleteCustomerCommand(Guid.NewGuid());

        var handler = new DeleteCustomerCommandHandler(
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
    public async Task Delete_InvalidCommand_ShouldReturnsFailResult()
    {
        // Arrange
        var handler = new DeleteCustomerCommandHandler(
            _validator,
            Mock.Of<ICustomerWriteOnlyRepository>(),
            Mock.Of<IUnitOfWork>());

        // Act
        var act = await handler.Handle(new DeleteCustomerCommand(Guid.Empty), CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.ValidationErrors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
    }
}