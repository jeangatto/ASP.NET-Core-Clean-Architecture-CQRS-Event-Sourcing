using System;
using Ardalis.Result;
using MediatR;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Customer.Queries;

public class GetCustomerByIdQuery(Guid id) : IRequest<Result<CustomerQueryModel>>
{
    public Guid Id { get; } = id;
}