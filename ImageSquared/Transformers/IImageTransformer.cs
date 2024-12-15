namespace ImageSquared.Transformers;

using System.Windows.Controls;

/// <summary>
/// The image transformer abstraction.
/// </summary>
internal interface IImageTransformer
{
    /// <summary>
    /// Transforms an image to its desired outcome.
    /// </summary>
    /// <param name="image">The original image.</param>
    /// <returns>The transformed image.</returns>
    Image Transform(Image image);
}
