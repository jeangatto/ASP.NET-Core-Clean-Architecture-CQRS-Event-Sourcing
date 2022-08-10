using System.ComponentModel.DataAnnotations;

namespace Shop.Core.AppSettings;

public sealed class ConnectionStrings
{
    [Required]
    public string ShopConnection { get; private init; }
    public string Collation { get; private init; }
}