namespace ImageSquared.Core;

using System;

/// <summary>
/// The naming strategy that uses timestamp.
/// </summary>
public sealed class TimestampNamingStrategy : IOutputNamingStrategy
{
    private readonly string fileExtension;
    private readonly string? prefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimestampNamingStrategy"/> class.
    /// </summary>
    /// <param name="fileExtension">The file extension.</param>
    /// <param name="prefix">The optional prefix.</param>
    public TimestampNamingStrategy(string fileExtension, string? prefix = null)
    {
        this.fileExtension = Guard.ThrowIfNullOrWhitespace(fileExtension);
        this.prefix = prefix;
    }

    /// <inheritdoc/>
    public string Generate()
    {
        if (!string.IsNullOrWhiteSpace(this.prefix))
        {
            return $"{this.prefix}{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.{this.fileExtension}";
        }

        return $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.{this.fileExtension}";
    }
}
