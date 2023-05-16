using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Shop.Core.Domain;

namespace Shop.Core.AppSettings;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class ConnectionOptions : IAppOptions
{
    public const string ConfigSectionPath = "ConnectionStrings";

    [Required] public string SqlConnection { get; private init; }

    [Required] public string NoSqlConnection { get; private init; }

    [Required] public string CacheConnection { get; private init; }
}