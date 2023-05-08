using System;

namespace Shop.Query.QueriesModel;

public class CustomerQueryModel : BaseQueryModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }

    public string FullName => (FirstName + " " + LastName).Trim();
}