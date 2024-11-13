namespace ImageSquared.Core.UnitTests;

using System;
using System.Text.RegularExpressions;
using Xunit;

public sealed partial class TimestampNamingStrategyTests
{
    private const string FileExtension = "png";

    [Fact]
    public void Constructor_WithEmptyFileExtension_Throws()
    {
        Assert.Throws<ArgumentException>(() => new TimestampNamingStrategy(string.Empty));
    }

    [Fact]
    public void Generate_WithPrefix_MatchPattern()
    {
        var regex = PrefixPattern();
        var naming = new TimestampNamingStrategy(FileExtension, "prefix-");

        var actual = naming.Generate();

        Assert.Matches(regex, actual);
    }

    [Fact]
    public void Generate_WithoutPrefix_MatchPattern()
    {
        var regex = NoPrefixPattern();
        var naming = new TimestampNamingStrategy(FileExtension);

        var actual = naming.Generate();

        Assert.Matches(regex, actual);
    }

    // Timestamp: 2024-11-13-22-51-23
    [GeneratedRegex("prefix-\\d{4}-\\d{2}-\\d{2}-\\d{2}-\\d{2}-\\d{2}.\\w{3,4}", RegexOptions.Compiled)]
    private static partial Regex PrefixPattern();

    [GeneratedRegex("\\d{4}-\\d{2}-\\d{2}-\\d{2}-\\d{2}-\\d{2}.\\w{3,4}", RegexOptions.Compiled)]
    private static partial Regex NoPrefixPattern();
}
