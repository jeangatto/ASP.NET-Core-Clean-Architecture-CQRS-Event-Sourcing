using System;
using Ardalis.Result;
using MediatR;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Customer.Queries;

public class GetCustomerByIdQuery : IRequest<Result<CustomerQueryModel>>
{
    public GetCustomerByIdQuery(Guid id) => Id = id;

    public Guid Id { get; }
}
