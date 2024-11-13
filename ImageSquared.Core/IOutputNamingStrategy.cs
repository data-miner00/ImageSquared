namespace ImageSquared.Core;

/// <summary>
/// The abstraction for image name generator.
/// </summary>
public interface IOutputNamingStrategy
{
    /// <summary>
    /// Generates a valid random image name with preconfigured settings.
    /// </summary>
    /// <returns>The randomly generated image name.</returns>
    string Generate();
}
