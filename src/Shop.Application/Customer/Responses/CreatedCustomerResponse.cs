using System;
using Shop.Core.SharedKernel;

namespace Shop.Application.Customer.Responses;

public class CreatedCustomerResponse(Guid id) : IResponse
{
    public Guid Id { get; } = id;
}