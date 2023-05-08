using System;
using Shop.Core.Abstractions;

namespace Shop.Application.Customer.Responses;

public class CreatedCustomerResponse : IResponse
{
    public CreatedCustomerResponse(Guid id) => Id = id;

    public Guid Id { get; }
}