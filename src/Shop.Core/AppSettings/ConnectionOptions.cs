using System.ComponentModel.DataAnnotations;
using Shop.Core.Abstractions;

namespace Shop.Core.AppSettings;

public sealed class ConnectionOptions : BaseOptions
{
    public const string ConfigSectionPath = "ConnectionStrings";

    /// <summary>
    /// String de conexão com a base de dados relacional.
    /// </summary>
    [Required]
    public string ShopConnection { get; private init; }

    /// <summary>
    /// String de conexão com a base de dados NoSql.
    /// </summary>
    [Required]
    public string EventConnection { get; private init; }
}