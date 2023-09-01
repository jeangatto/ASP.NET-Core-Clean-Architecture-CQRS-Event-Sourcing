using System;
using System.ComponentModel.DataAnnotations;
using Shop.Core.SharedKernel;

namespace Shop.Core.AppSettings;

public sealed class ConnectionOptions : IAppOptions
{
    static string IAppOptions.ConfigSectionPath => "ConnectionStrings";
    private const StringComparison ComparisonType = StringComparison.InvariantCultureIgnoreCase;

    [Required]
    public string SqlConnection { get; private init; }

    [Required]
    public string NoSqlConnection { get; private init; }

    [Required]
    public string CacheConnection { get; private init; }

    public bool CacheConnectionInMemory() => CacheConnection.Equals("InMemory", ComparisonType);
}