using System;
using Shop.Query.Abstractions;

namespace Shop.Query.QueriesModel;

public abstract class BaseQueryModel : IQueryModel<Guid>
{
    public Guid Id { get; } = Guid.NewGuid();
}