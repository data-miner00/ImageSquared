namespace ImageSquared.Core.UnitTests;

using ImageSquared.Core.Models;
using System;
using Xunit;

public sealed class OutputNamingStrategyFactoryTests
{
    [Fact]
    public void Constructor_InvalidImageFormat_Throws()
    {
        Assert.Throws<ArgumentException>(() => new OutputNamingStrategyFactory(ImageFormat.None));
    }

    [Theory]
    [InlineData(NamingStrategy.Guid, typeof(GuidNamingStrategy))]
    [InlineData(NamingStrategy.Timestamp, typeof(TimestampNamingStrategy))]
    public void Create_ValidNamingStrategy_InstanceCreatedAccordingly(NamingStrategy namingStrategy, Type namingStrategyImplType)
    {
        var factory = new OutputNamingStrategyFactory(ImageFormat.Jpg);

        var namingStrategyImpl = factory.Create(namingStrategy);

        Assert.IsType(namingStrategyImplType, namingStrategyImpl);
    }

    [Fact]
    public void Create_InvalidNamingStrategy_Throws()
    {
        var factory = new OutputNamingStrategyFactory(ImageFormat.Png);

        Assert.Throws<NotSupportedException>(() => factory.Create(default));
    }
}
