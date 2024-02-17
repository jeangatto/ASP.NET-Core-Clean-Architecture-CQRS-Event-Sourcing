using System;
using Shop.Core.SharedKernel;

namespace Shop.Application.Product.Responses;

public class CreatedProductResponse(Guid id) : IResponse
{
    public Guid Id { get; } = id;
}