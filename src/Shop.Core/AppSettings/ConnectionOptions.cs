using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Shop.Core.Shared;

namespace Shop.Core.AppSettings;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class ConnectionOptions : IAppOptions
{
    public const string ConfigSectionPath = "ConnectionStrings";

    [Required] public string SqlConnection { get; private init; }

    [Required] public string NoSqlConnection { get; private init; }

    [Required] public string CacheConnection { get; private init; }
}