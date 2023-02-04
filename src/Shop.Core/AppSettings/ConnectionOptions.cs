using System.ComponentModel.DataAnnotations;
using Shop.Core.Abstractions;

namespace Shop.Core.AppSettings;

public sealed class ConnectionOptions : BaseOptions
{
    public const string ConfigSectionPath = "ConnectionStrings";

    [Required]
    public string ShopConnection { get; private init; }

    [Required]
    public string EventConnection { get; private init; }

    public string Collation { get; private init; }
}