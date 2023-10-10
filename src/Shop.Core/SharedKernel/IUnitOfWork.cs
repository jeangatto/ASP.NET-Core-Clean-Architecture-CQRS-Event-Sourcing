using System;
using System.Threading.Tasks;

namespace Shop.Core.SharedKernel;

/// <summary>
/// Represents a unit of work for managing database operations.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Saves the changes made in the unit of work asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveChangesAsync();
}