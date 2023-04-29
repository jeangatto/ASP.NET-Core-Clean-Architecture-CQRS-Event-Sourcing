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
using CustomerAggregate = Shop.Domain.Entities.CustomerAggregate;

namespace Shop.UnitTests.Application.Customer.Handlers;

[UnitTest]
public class CreateCustomerCommandHandlerTests
{
    [Fact]
    public async Task Add_ValidCommand_ShouldReturnsSuccessResult()
    {
        // Arrange
        var command = new Faker<CreateCustomerCommand>()
            .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
            .RuleFor(command => command.LastName, faker => faker.Person.LastName)
            .RuleFor(command => command.Gender, faker => faker.PickRandom<EGender>())
            .RuleFor(command => command.Email, faker => faker.Person.Email.ToLowerInvariant())
            .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
            .Generate();

        var repositoryMock = new Mock<ICustomerWriteOnlyRepository>();
        repositoryMock
            .Setup(s => s.ExistsByEmailAsync(It.Is<Email>(email => email.Address == command.Email)))
            .ReturnsAsync(false)
            .Verifiable();

        repositoryMock
            .Setup(s => s.Add(It.Is<CustomerAggregate.Customer>(entity =>
                entity.FirstName == command.FirstName
                && entity.LastName == command.LastName
                && entity.Gender == command.Gender
                && entity.Email.Address == command.Email
                && entity.DateOfBirth == command.DateOfBirth)))
            .Verifiable();

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(s => s.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        var handler = new CreateCustomerCommandHandler(
            new CreateCustomerCommandValidator(),
            repositoryMock.Object,
            uowMock.Object);

        // Act
        var act = await handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.SuccessMessage.Should().Be("Cadastrado com sucesso!");
        act.Value.Should().NotBeNull();
        act.Value.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Add_DuplicateEmailCommand_ShouldReturnsFailResult()
    {
        // Arrange
        var command = new Faker<CreateCustomerCommand>()
            .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
            .RuleFor(command => command.LastName, faker => faker.Person.LastName)
            .RuleFor(command => command.Gender, faker => faker.PickRandom<EGender>())
            .RuleFor(command => command.Email, faker => faker.Person.Email.ToLowerInvariant())
            .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
            .Generate();

        var repositoryMock = new Mock<ICustomerWriteOnlyRepository>();
        repositoryMock
            .Setup(s => s.ExistsByEmailAsync(It.Is<Email>(email => email.Address == command.Email)))
            .ReturnsAsync(true)
            .Verifiable();

        var handler = new CreateCustomerCommandHandler(
            new CreateCustomerCommandValidator(),
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
    public async Task Add_InvalidCommand_ShouldReturnsFailResult()
    {
        // Arrange
        var handler = new CreateCustomerCommandHandler(
            new CreateCustomerCommandValidator(),
            Mock.Of<ICustomerWriteOnlyRepository>(),
            Mock.Of<IUnitOfWork>());

        // Act
        var act = await handler.Handle(new CreateCustomerCommand(), CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.ValidationErrors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
    }
}