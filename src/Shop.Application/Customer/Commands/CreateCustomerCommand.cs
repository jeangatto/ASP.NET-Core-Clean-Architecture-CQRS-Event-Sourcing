using System;
using Shop.Core.Interfaces;

namespace Shop.Application.Commands;

public class CreateCustomerCommand : ICommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}