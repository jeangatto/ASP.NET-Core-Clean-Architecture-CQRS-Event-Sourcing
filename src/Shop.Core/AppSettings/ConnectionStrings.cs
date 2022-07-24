using System.ComponentModel.DataAnnotations;

namespace Shop.Core.AppSettings;

public sealed class ConnectionStrings
{
    [Required]
    public string ShopConnection { get; private set; }
    public string Collation { get; private set; }
}