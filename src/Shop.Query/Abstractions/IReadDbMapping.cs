namespace Shop.Query.Abstractions;

/// <summary>
/// Represents an interface for reading database mappings.
/// </summary>
public interface IReadDbMapping
{
    /// <summary>
    /// Configures the mappings for reading from the database.
    /// </summary>
    void Configure();
}