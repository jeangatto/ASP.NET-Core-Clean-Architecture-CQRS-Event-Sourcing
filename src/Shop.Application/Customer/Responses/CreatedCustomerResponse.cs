using System;
using Shop.Core.Common;

namespace Shop.Application.Customer.Responses;

public class CreatedCustomerResponse : BaseResponse
{
    public CreatedCustomerResponse(Guid id) => Id = id;

    public Guid Id { get; }
}