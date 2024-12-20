﻿namespace ImageSquared.ViewModel;

using ImageSquared.Core;
using ImageSquared.Core.Repositories;
using ImageSquared.View;
using System.Collections.ObjectModel;

/// <summary>
/// The view model for <see cref="HistoryView"/>.
/// </summary>
public sealed class HistoryViewModel : ViewModel
{
    private readonly IHistoryRepository repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryViewModel"/> class.
    /// </summary>
    /// <param name="repository">The history repository.</param>
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
