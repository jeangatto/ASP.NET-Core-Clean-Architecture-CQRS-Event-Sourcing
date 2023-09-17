using System;
using Ardalis.Result;
using MediatR;

namespace Shop.Application.Customer.Commands;

public class DeleteCustomerCommand : IRequest<Result>
{
    public DeleteCustomerCommand(Guid id) => Id = id;

    public Guid Id { get; }
}
