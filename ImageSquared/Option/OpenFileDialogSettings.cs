namespace ImageSquared.Option;

/// <summary>
/// The settings for open file dialog.
/// </summary>
public sealed class OpenFileDialogSettings
{
    /// <summary>
    /// Gets or sets the filter to be applied when opening dialog.
    /// </summary>
    public string Filter { get; set; }

    /// <summary>
    /// Gets or sets the title for the dialog.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog should be multiselect.
    /// </summary>
    public bool Multiselect { get; set; }
}
