using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Moq;
using Shop.Application.Customer.Commands;
using Shop.Application.Customer.Handlers;
using Shop.Core.Abstractions;
using Shop.Core.ValueObjects;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.Entities.CustomerAggregate.Repositories;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Application.Customer.Handlers;

[UnitTest]
public class DeleteCustomerCommandHandlerTests
{
    [Fact]
    public async Task Delete_ValidCustomerId_ShouldReturnsSuccessResult()
    {
        // Arrange
        var customerEntity = new Faker<Domain.Entities.CustomerAggregate.Customer>()
            .CustomInstantiator(f => new Domain.Entities.CustomerAggregate.Customer(
                f.Person.FirstName,
                f.Person.LastName,
                f.PickRandom<EGender>(),
                new Email(f.Person.Email),
                f.Person.DateOfBirth))
            .Generate();

        var command = new DeleteCustomerCommand(customerEntity.Id);

        var repositoryMock = new Mock<ICustomerWriteOnlyRepository>();
        repositoryMock
            .Setup(s => s.GetByIdAsync(It.Is<Guid>(id => id == command.Id)))
            .ReturnsAsync(customerEntity)
            .Verifiable();

        repositoryMock
            .Setup(s => s.Remove(It.Is<Domain.Entities.CustomerAggregate.Customer>(entity => entity == customerEntity)))
            .Verifiable();

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(s => s.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        var handler = new DeleteCustomerCommandHandler(
            new DeleteCustomerCommandValidator(),
            repositoryMock.Object,
            uowMock.Object);

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

        var repositoryMock = new Mock<ICustomerWriteOnlyRepository>();
        repositoryMock
            .Setup(s => s.GetByIdAsync(It.Is<Guid>(id => id == command.Id)))
            .ReturnsAsync((Domain.Entities.CustomerAggregate.Customer)null)
            .Verifiable();

        var handler = new DeleteCustomerCommandHandler(
            new DeleteCustomerCommandValidator(),
            repositoryMock.Object,
            Mock.Of<IUnitOfWork>());

        // Act
        var act = await handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Errors.Should().NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.Contain(errorMessage => errorMessage == $"Nenhum cliente encontrado pelo Id: {command.Id}");
    }

    [Fact]
    public async Task Delete_InvalidCommand_ShouldReturnsFailResult()
    {
        // Arrange
        var handler = new DeleteCustomerCommandHandler(
            new DeleteCustomerCommandValidator(),
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