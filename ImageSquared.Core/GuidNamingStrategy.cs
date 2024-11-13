namespace ImageSquared.Core;

using System;

/// <summary>
/// The naming strategy that uses <see cref="Guid"/>.
/// </summary>
public sealed class GuidNamingStrategy : IOutputNamingStrategy
{
    private readonly string fileExtension;
    private readonly string? prefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuidNamingStrategy"/> class.
    /// </summary>
    /// <param name="fileExtension">The file extension.</param>
    /// <param name="prefix">The optional prefix.</param>
    public GuidNamingStrategy(string fileExtension, string? prefix = null)
    {
        this.fileExtension = fileExtension;
        this.prefix = prefix;
    }

    /// <inheritdoc/>
    public string Generate()
    {
        if (!string.IsNullOrEmpty(this.prefix))
        {
            return $"{this.prefix}{Guid.NewGuid()}.{this.fileExtension}";
        }

        return $"{Guid.NewGuid()}.{this.fileExtension}";
    }
}
