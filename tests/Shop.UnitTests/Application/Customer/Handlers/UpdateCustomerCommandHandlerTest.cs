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
using Xunit;
using Xunit.Categories;
using CustomerAggregate = Shop.Domain.Entities.CustomerAggregate;

namespace Shop.UnitTests.Application.Customer.Handlers;

[UnitTest]
public class UpdateCustomerCommandHandlerTest
{
    [Fact]
    public async Task Update_ValidCommand_ShouldReturnsSuccessResult()
    {
        // Arrange
        var customerEntity = new Faker<CustomerAggregate.Customer>()
            .CustomInstantiator(faker => new CustomerAggregate.Customer(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.PickRandom<EGender>(),
                new Email(faker.Person.Email),
                faker.Person.DateOfBirth))
            .Generate();

        var command = new Faker<UpdateCustomerCommand>()
            .RuleFor(command => command.Id, customerEntity.Id)
            .RuleFor(command => command.Email, faker => faker.Person.Email.ToLowerInvariant())
            .Generate();

        var repositoryMock = new Mock<ICustomerWriteOnlyRepository>();
        repositoryMock
            .Setup(s => s.ExistsByEmailAsync(
                It.Is<Email>(email => email.Address == command.Email),
                It.Is<Guid>(id => id == command.Id)))
            .ReturnsAsync(false)
            .Verifiable();

        repositoryMock
            .Setup(s => s.GetByIdAsync(It.Is<Guid>(id => id == command.Id)))
            .ReturnsAsync(customerEntity)
            .Verifiable();

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(s => s.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        var handler = new UpdateCustomerCommandHandler(
            new UpdateCustomerCommandValidator(),
            repositoryMock.Object,
            uowMock.Object);

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
        var customerEntity = new Faker<CustomerAggregate.Customer>()
            .CustomInstantiator(faker => new CustomerAggregate.Customer(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.PickRandom<EGender>(),
                new Email(faker.Person.Email),
                faker.Person.DateOfBirth))
            .Generate();

        var command = new Faker<UpdateCustomerCommand>()
            .RuleFor(command => command.Id, customerEntity.Id)
            .RuleFor(command => command.Email, faker => faker.Person.Email.ToLowerInvariant())
            .Generate();

        var repositoryMock = new Mock<ICustomerWriteOnlyRepository>();
        repositoryMock
            .Setup(s => s.ExistsByEmailAsync(
                It.Is<Email>(email => email.Address == command.Email),
                It.Is<Guid>(id => id == command.Id)))
            .ReturnsAsync(true)
            .Verifiable();

        repositoryMock
            .Setup(s => s.GetByIdAsync(It.Is<Guid>(id => id == command.Id)))
            .ReturnsAsync(customerEntity)
            .Verifiable();

        var handler = new UpdateCustomerCommandHandler(
            new UpdateCustomerCommandValidator(),
            repositoryMock.Object,
            Mock.Of<IUnitOfWork>());

        // Act
        var act = await handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Errors.Should().NotBeNullOrEmpty()
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

        var repositoryMock = new Mock<ICustomerWriteOnlyRepository>();
        repositoryMock
            .Setup(s => s.GetByIdAsync(It.Is<Guid>(id => id == command.Id)))
            .ReturnsAsync((CustomerAggregate.Customer)null)
            .Verifiable();

        var handler = new UpdateCustomerCommandHandler(
            new UpdateCustomerCommandValidator(),
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
    public async Task Update_InvalidCommand_ShouldReturnsFailResult()
    {
        // Arrange
        var handler = new UpdateCustomerCommandHandler(
            new UpdateCustomerCommandValidator(),
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