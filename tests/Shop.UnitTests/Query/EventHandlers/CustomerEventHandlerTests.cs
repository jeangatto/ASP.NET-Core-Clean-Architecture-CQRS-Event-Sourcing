using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Query.Abstractions;
using Shop.Query.Application.Customer.Queries;
using Shop.Query.EventHandlers;
using Shop.Query.QueriesModel;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Query.EventHandlers;

[UnitTest]
public class CustomerEventHandlerTests
{
    private static readonly DateTime DateOfBirth = new(1990, 1, 1);

    [Fact]
    public async Task Handle_CustomerCreated_ShouldUpsertModelAndClearCache()
    {
        var id = Guid.NewGuid();
        var customer = CreateModel(id);
        var notification = CreateCreatedEvent(id);
        var mapper = Substitute.For<IMapper>();
        mapper.Map<CustomerQueryModel>(notification).Returns(customer);
        var synchronizeDb = Substitute.For<ISynchronizeDb>();
        var cacheService = Substitute.For<ICacheService>();
        var handler = CreateHandler(mapper, synchronizeDb, cacheService);

        await handler.Handle(notification, CancellationToken.None);

        await synchronizeDb.Received(1).UpsertAsync(
            customer, Arg.Any<Expression<Func<CustomerQueryModel, bool>>>());
        await VerifyCacheWasCleared(cacheService, id);
    }

    [Fact]
    public async Task Handle_CustomerUpdated_ShouldUpsertModelAndClearCache()
    {
        var id = Guid.NewGuid();
        var customer = CreateModel(id);
        var notification = CreateUpdatedEvent(id);
        var mapper = Substitute.For<IMapper>();
        mapper.Map<CustomerQueryModel>(notification).Returns(customer);
        var synchronizeDb = Substitute.For<ISynchronizeDb>();
        var cacheService = Substitute.For<ICacheService>();
        var handler = CreateHandler(mapper, synchronizeDb, cacheService);

        await handler.Handle(notification, CancellationToken.None);

        await synchronizeDb.Received(1).UpsertAsync(
            customer, Arg.Any<Expression<Func<CustomerQueryModel, bool>>>());
        await VerifyCacheWasCleared(cacheService, id);
    }

    [Fact]
    public async Task Handle_CustomerDeleted_ShouldDeleteModelAndClearCache()
    {
        var id = Guid.NewGuid();
        var notification = CreateDeletedEvent(id);
        var synchronizeDb = Substitute.For<ISynchronizeDb>();
        var cacheService = Substitute.For<ICacheService>();
        var handler = CreateHandler(Substitute.For<IMapper>(), synchronizeDb, cacheService);
        var model = CreateModel(id);

        await handler.Handle(notification, CancellationToken.None);

        await synchronizeDb.Received(1).DeleteAsync(
            Arg.Is<Expression<Func<CustomerQueryModel, bool>>>(filter => filter.Compile()(model)));
        await VerifyCacheWasCleared(cacheService, id);
    }

    private static CustomerEventHandler CreateHandler(
        IMapper mapper,
        ISynchronizeDb synchronizeDb,
        ICacheService cacheService) =>
        new(mapper, synchronizeDb, cacheService, Substitute.For<ILogger<CustomerEventHandler>>());

    private static CustomerQueryModel CreateModel(Guid id) =>
        new(id, "John", "Doe", "Male", "john.doe@example.com", DateOfBirth);

    private static CustomerCreatedEvent CreateCreatedEvent(Guid id) =>
        new(id, "John", "Doe", EGender.Male, "john.doe@example.com", DateOfBirth);

    private static CustomerUpdatedEvent CreateUpdatedEvent(Guid id) =>
        new(id, "John", "Doe", EGender.Male, "john.doe@example.com", DateOfBirth);

    private static CustomerDeletedEvent CreateDeletedEvent(Guid id) =>
        new(id, "John", "Doe", EGender.Male, "john.doe@example.com", DateOfBirth);

    private static async Task VerifyCacheWasCleared(ICacheService cacheService, Guid id)
    {
        var expectedKeys = new[] { nameof(GetAllCustomerQuery), $"{nameof(GetCustomerByIdQuery)}_{id}" };
        await cacheService.Received(1).RemoveAsync(
            Arg.Is<string[]>(keys => keys.SequenceEqual(expectedKeys)));
    }
}
