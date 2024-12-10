namespace ImageSquared.ViewModel;

using ImageSquared.Core;
using ImageSquared.Core.Repositories;
using System.Collections.ObjectModel;

public sealed class HistoryViewModel : ViewModel
{
    private readonly IHistoryRepository repository;

    public HistoryViewModel(IHistoryRepository repository)
    {
        this.repository = Guard.ThrowIfNull(repository);
        this.LoadHistoryFile();
    }

    /// <summary>
    /// Gets the observable file history.
    /// </summary>
    public ObservableCollection<string> FileHistory { get; } = [];

    private void LoadHistoryFile()
    {
        var fileNames = this.repository.GetAllAsync().GetAwaiter().GetResult();

        foreach (var filePath in fileNames)
        {
            this.FileHistory.Add(filePath);
        }
    }
}
