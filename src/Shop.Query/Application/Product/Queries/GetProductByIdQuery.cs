using System;
using Ardalis.Result;
using MediatR;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Product.Queries;

public class GetProductByIdQuery(Guid id) : IRequest<Result<ProductQueryModel>>
{
    public Guid Id { get; } = id;
}