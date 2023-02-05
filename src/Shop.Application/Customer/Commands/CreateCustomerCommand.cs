using System;
using Ardalis.Result;
using MediatR;
using Shop.Application.Customer.Responses;

namespace Shop.Application.Commands;

public class CreateCustomerCommand : IRequest<Result<CreatedCustomerResponse>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}