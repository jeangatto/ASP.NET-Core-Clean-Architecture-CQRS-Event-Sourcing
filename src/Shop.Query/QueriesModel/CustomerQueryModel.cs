using System;
using Shop.Query.Abstractions;

namespace Shop.Query.QueriesModel;

public class CustomerQueryModel : IQueryModel<Guid>
{
    public CustomerQueryModel(
        Guid id,
        string firstName,
        string lastName,
        string gender,
        string email,
        DateTime dateOfBirth)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        Email = email;
        DateOfBirth = dateOfBirth;
    }

    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Gender { get; }
    public string Email { get; }
    public DateTime DateOfBirth { get; }

    public string FullName => (FirstName + " " + LastName).Trim();
}