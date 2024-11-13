namespace ImageSquared.Core.UnitTests;

using System;
using System.Text.RegularExpressions;
using Xunit;

public sealed partial class GuidNamingStrategyTests
{
    private const string FileExtension = "png";

    [Fact]
    public void Constructor_WithEmptyFileExtension_Throws()
    {
        Assert.Throws<ArgumentException>(() => new GuidNamingStrategy(string.Empty));
    }

    [Fact]
    public void Generate_WithPrefix_MatchPattern()
    {
        var regex = PrefixPattern();
        var naming = new GuidNamingStrategy(FileExtension, "prefix-");

        var actual = naming.Generate();

        Assert.Matches(regex, actual);
    }

    [Fact]
    public void Generate_WithoutPrefix_MatchPattern()
    {
        var regex = NoPrefixPattern();
        var naming = new GuidNamingStrategy(FileExtension);

        var actual = naming.Generate();

        Assert.Matches(regex, actual);
    }

    // Guid: 6c56c5a9-903c-464f-8624-d71526393926
    [GeneratedRegex("prefix-[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}.\\w{3,4}", RegexOptions.Compiled)]
    private static partial Regex PrefixPattern();

    [GeneratedRegex("[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}.\\w{3,4}", RegexOptions.Compiled)]
    private static partial Regex NoPrefixPattern();
}
