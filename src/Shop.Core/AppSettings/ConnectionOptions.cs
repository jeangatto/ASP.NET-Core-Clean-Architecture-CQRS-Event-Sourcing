using System.ComponentModel.DataAnnotations;

namespace Shop.Core.AppSettings;

public sealed class ConnectionOptions : BaseOptions
{
    public const string ConfigSectionPath = "ConnectionStrings";

    [Required]
    public string ShopConnection { get; private init; }
    public string Collation { get; private init; }
}