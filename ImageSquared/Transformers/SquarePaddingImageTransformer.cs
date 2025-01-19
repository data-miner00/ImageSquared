namespace ImageSquared.Transformers;

using ImageSquared.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// <summary>
/// Adds padding to a portrait or landscape image into square.
/// </summary>
internal sealed class SquarePaddingImageTransformer : IImageTransformer
{
    private readonly int dpiX = 96;
    private readonly int dpiY = 96;

    /// <inheritdoc/>
    public Image Transform(Image image)
    {
        var originalHeight = Convert.ToInt32(image.Source.Height);
        var originalWidth = Convert.ToInt32(image.Source.Width);
        var imageOrientation = GetImageOrientation(originalHeight, originalWidth);
        var standardLength = CalculateStandardizedLength(originalHeight, originalWidth);
        var extendedImage = new RenderTargetBitmap(standardLength, standardLength, this.dpiX, this.dpiY, PixelFormats.Pbgra32);

        var visual = new DrawingVisual();

        using (var drawingContext = visual.RenderOpen())
        {
            if (imageOrientation == ImageOrientation.Portrait)
            {
                var startingPosition = (standardLength / 2) - (originalWidth / 2);
                drawingContext.DrawImage(image.Source, new Rect(startingPosition, 0, originalWidth, originalHeight));
            }
            else if (imageOrientation == ImageOrientation.Landscape)
            {
                var startingPosition = (standardLength / 2) - (originalHeight / 2);
                drawingContext.DrawImage(image.Source, new Rect(0, startingPosition, originalWidth, originalHeight));
            }
            else
            {
                drawingContext.DrawImage(image.Source, new Rect(0, 0, originalWidth, originalHeight));
            }
        }

        extendedImage.Render(visual);

        var transformedImage = new Image
        {
            Source = extendedImage,
        };

        return transformedImage;
    }

    private static int CalculateStandardizedLength(int height, int width)
    {
        return height >= width ? height : width;
    }

    private static ImageOrientation GetImageOrientation(int height, int width)
    {
        var delta = height - width;

        ImageOrientation imageOrientation;

        if (delta > 0)
        {
            imageOrientation = ImageOrientation.Portrait;
        }
        else if (delta < 0)
        {
            imageOrientation = ImageOrientation.Landscape;
        }
        else
        {
            imageOrientation = ImageOrientation.Squared;
        }

        return imageOrientation;
    }
}
