namespace ImageSquared.Option;

using ImageSquared.Core.Models;

public sealed class OutputSettings
{
    public ImageFormat ImageFormat { get; set; }

    public NamingStrategy NamingStrategy { get; set; }

    public string? Prefix { get; set; }
}
