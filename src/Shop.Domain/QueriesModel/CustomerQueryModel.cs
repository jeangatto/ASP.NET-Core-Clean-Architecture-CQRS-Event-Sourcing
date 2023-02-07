using System;
using Shop.Core.Interfaces;
using Shop.Domain.Enums;

namespace Shop.Domain.QueriesModel;

public class CustomerQueryModel : IQueryModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public EGender Gender { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}