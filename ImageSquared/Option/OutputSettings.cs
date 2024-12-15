namespace ImageSquared.Option;

using ImageSquared.Core.Models;

/// <summary>
/// The file output configurations.
/// </summary>
public sealed class OutputSettings
{
    /// <summary>
    /// Gets or sets the image format to be exported.
    /// </summary>
    public ImageFormat ImageFormat { get; set; }

    /// <summary>
    /// Gets or sets the naming strategy of the exported image file.
    /// </summary>
    public NamingStrategy NamingStrategy { get; set; }

    /// <summary>
    /// Gets or sets the optional naming prefix for the image file.
    /// </summary>
    public string? Prefix { get; set; }
}
