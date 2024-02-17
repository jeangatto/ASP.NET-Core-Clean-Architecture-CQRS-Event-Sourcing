using System;
using Ardalis.Result;
using MediatR;

namespace Shop.Application.Product.Commands;

public class DeleteProductCommand(Guid id) : IRequest<Result>
{
    public Guid Id { get; } = id;
}