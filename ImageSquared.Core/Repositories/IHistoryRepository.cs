namespace ImageSquared.Core.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// The abstraction for history repository.
/// </summary>
/// <typeparam name="T">The type of history object.</typeparam>
public interface IHistoryRepository<T>
{
    /// <summary>
    /// Gets all the existing entries in the history.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of entries.</returns>
    public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an entry into the history for persistence.
    /// </summary>
    /// <param name="filePath">The file path entry to be added.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    public Task AddAsync(string filePath, CancellationToken cancellationToken = default);
}
