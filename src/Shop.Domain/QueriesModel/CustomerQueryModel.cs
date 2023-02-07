using System;
using Shop.Core.Abstractions;
using Shop.Domain.Enums;

namespace Shop.Domain.QueriesModel;

public class CustomerQueryModel : BaseQueryModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public EGender Gender { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}