namespace ImageSquared.ViewModel;

using ImageSquared.Core;
using ImageSquared.View;

/// <summary>
/// The view model for <see cref="MainWindow"/>.
/// </summary>
internal sealed class MainWindowViewModel : ViewModel
{
    private int currentImageWidth;
    private int currentImageHeight;
    private int standardizedLength;

    /// <summary>
    /// Gets or sets the current image height.
    /// </summary>
    public int CurrentImageHeight
    {
        get => this.currentImageHeight;
        set
        {
            this.currentImageHeight = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the current image width.
    /// </summary>
    public int CurrentImageWidth
    {
        get => this.currentImageWidth;
        set
        {
            this.currentImageWidth = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the standardized length of the new image.
    /// </summary>
    public int StandardizedLength
    {
        get => this.standardizedLength;
        set
        {
            this.standardizedLength = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the image orientation based on current width and current length.
    /// </summary>
    public ImageOrientation ImageOrientation
    {
        get
        {
            var delta = this.currentImageHeight - this.currentImageWidth;

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
}
