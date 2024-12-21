namespace ImageSquared.ViewModel;

using ImageSquared.Core;
using ImageSquared.Core.Models;
using ImageSquared.Core.Repositories;
using ImageSquared.View;
using System.Collections.ObjectModel;

/// <summary>
/// The view model for <see cref="HistoryView"/>.
/// </summary>
public sealed class HistoryViewModel : ViewModel
{
    private readonly IHistoryRepository<LoadHistoryRecord> repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryViewModel"/> class.
    /// </summary>
    /// <param name="repository">The history repository.</param>
    public HistoryViewModel(IHistoryRepository<LoadHistoryRecord> repository)
    {
        this.repository = Guard.ThrowIfNull(repository);
        this.LoadHistoryFile();
    }

    /// <summary>
    /// Gets the observable file history.
    /// </summary>
    public ObservableCollection<LoadHistoryRecord> FileHistory { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether to show the history list.
    /// </summary>
    public bool ShowHistory { get; set; } = true;

    private void LoadHistoryFile()
    {
        var fileNames = this.repository.GetAllAsync().GetAwaiter().GetResult();

        foreach (var filePath in fileNames)
        {
            this.FileHistory.Add(filePath);
        }
    }
}
