namespace ImageSquared.Option;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// The user configurable settings for the program.
/// </summary>
public sealed class DefaultSettings
{
    /// <summary>
    /// Todo.
    /// </summary>
    [Range(0, 100)]
    public int SimilarityPercentageThreshold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to launch the application in debug mode.
    /// </summary>
    public bool Debug { get; set; }

    /// <summary>
    /// Gets or sets the image output folder.
    /// </summary>
    public string StorageFolderPath { get; set; }

    /// <summary>
    /// Gets or sets the history file path.
    /// </summary>
    public string HistoryFilePath { get; set; }
}
