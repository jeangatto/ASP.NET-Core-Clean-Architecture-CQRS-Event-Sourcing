using System;
using Shop.Core;

namespace Shop.Application.Customer.Responses;

public class CreatedCustomerResponse : BaseResponse
{
    public CreatedCustomerResponse(Guid id) => Id = id;

    public Guid Id { get; }
}