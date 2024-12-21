namespace ImageSquared.Integrations.Repositories;

using ImageSquared.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageSquared.Core;

/// <summary>
/// The file-based history repository implementation.
/// </summary>
public sealed class FileHistoryRepository : IHistoryRepository<string>
{
    private readonly string historyFilePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileHistoryRepository"/> class.
    /// </summary>
    /// <param name="historyFilePath">The history file path.</param>
    public FileHistoryRepository(string historyFilePath)
    {
        this.historyFilePath = Guard.ThrowIfNullOrWhitespace(historyFilePath);
    }

    /// <inheritdoc/>
    public Task AddAsync(string filePath, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var stream = File.AppendText(this.historyFilePath);

        return stream.WriteLineAsync(filePath);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<string>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(this.historyFilePath))
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }

        // bug, ReadAllLinesAsync will stuck here
        var lines = File.ReadAllLines(this.historyFilePath);

        return Task.FromResult<IEnumerable<string>>(lines);
    }
}
