using System.ComponentModel.DataAnnotations;

namespace Shop.Core.AppSettings;

public class ConnectionStrings
{
    [Required]
    public string ShopConnection { get; private set; }
    public string Collation { get; private set; }
}