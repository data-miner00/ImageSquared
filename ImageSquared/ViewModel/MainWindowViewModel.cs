namespace ImageSquared.ViewModel;

using ImageSquared.Core.Models;
using ImageSquared.View;
using System.Windows.Input;
using System.Windows.Media.Imaging;

/// <summary>
/// The view model for <see cref="MainWindow"/>.
/// </summary>
internal sealed class MainWindowViewModel : ViewModel
{
    private int currentImageWidth;
    private int currentImageHeight;
    private int standardizedLength;
    private BitmapImage? currentImage;
    private RenderTargetBitmap? transformedBitmapImage;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="selectImageAction">The action to select an image.</param>
    public MainWindowViewModel(Action selectImageAction)
    {
        this.SelectImageCommand = new RelayCommand(_ => selectImageAction());
    }

    /// <summary>
    /// Gets the command to select an image.
    /// </summary>
    public ICommand SelectImageCommand { get; }

    public string TestText => "Hello world";

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

    /// <summary>
    /// Gets or sets the current image selected by the user.
    /// </summary>
    public BitmapImage? CurrentImage
    {
        get => this.currentImage;
        set
        {
            this.currentImage = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the transformed image from <see cref="CurrentImage"/>.
    /// </summary>
    public RenderTargetBitmap? TransformedBitmapImage
    {
        get => this.transformedBitmapImage;
        set
        {
            this.transformedBitmapImage = value;
            this.OnPropertyChanged();
        }
    }
}
