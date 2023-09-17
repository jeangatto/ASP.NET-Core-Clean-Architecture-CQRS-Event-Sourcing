namespace Shop.Core.SharedKernel;

/// <summary>
/// Represents the interface for application options.
/// </summary>
public interface IAppOptions
{
    /// <summary>
    /// The configuration section path.
    /// </summary>
    static abstract string ConfigSectionPath { get; }
}
