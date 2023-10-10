namespace Shop.Core.SharedKernel.Correlation;

/// <summary>
/// Represents a correlation ID generator.
/// </summary>
public interface ICorrelationIdGenerator
{
    /// <summary>
    /// Generates a correlation ID.
    /// </summary>
    /// <returns>The generated correlation ID.</returns>
    string Get();

    /// <summary>
    /// Sets the correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID to set.</param>
    void Set(string correlationId);
}