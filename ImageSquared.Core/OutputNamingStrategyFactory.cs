namespace ImageSquared.Core;

using ImageSquared.Core.Models;
using System;

/// <summary>
/// The factory for constructing the concrete <see cref="IOutputNamingStrategy"/> class.
/// </summary>
public sealed class OutputNamingStrategyFactory
{
    private readonly string fileExtension;
    private readonly string? prefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputNamingStrategyFactory"/> class.
    /// </summary>
    /// <param name="imageFormat">The desired image format.</param>
    /// <param name="prefix">The optional prefix.</param>
    public OutputNamingStrategyFactory(ImageFormat imageFormat, string? prefix = null)
    {
        Guard.ThrowIfDefault(imageFormat);
        this.fileExtension = imageFormat.ToString().ToLowerInvariant();
        this.prefix = prefix;
    }

    /// <summary>
    /// Creates a concrete instance of <see cref="IOutputNamingStrategy"/>.
    /// </summary>
    /// <param name="namingStrategy">The naming strategy intended.</param>
    /// <returns>The concrete instance for naming strategy.</returns>
    /// <exception cref="NotSupportedException">Throws when <see cref="NamingStrategy"/> provided is not supported.</exception>
    public IOutputNamingStrategy Create(NamingStrategy namingStrategy)
    {
        return namingStrategy switch
        {
            NamingStrategy.Guid => new GuidNamingStrategy(this.fileExtension, this.prefix),
            NamingStrategy.Timestamp => new TimestampNamingStrategy(this.fileExtension, this.prefix),
            _ => throw new NotSupportedException(),
        };
    }
}
