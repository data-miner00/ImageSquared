namespace ImageSquared.ViewModel;

using ImageSquared.Core.Models;
using ImageSquared.View;
using System.Windows.Input;
using System.Windows.Media.Imaging;

/// <summary>
/// The view model for <see cref="MainWindow"/>.
/// </summary>
public sealed class MainWindowViewModel : ViewModel
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
    /// <param name="convertImageAction">The action to convert an image.</param>
    /// <param name="resetAction">The action to reset the states.</param>
    public MainWindowViewModel(Action selectImageAction, Action convertImageAction, Action resetAction)
    {
        this.SelectImageCommand = new RelayCommand(_ => selectImageAction());
        this.ConvertImageCommand = new RelayCommand(_ => convertImageAction(), (_) => this.currentImage is not null);
        this.ClearSelectedImageCommand = new RelayCommand(_ => resetAction(), (_) => this.currentImage is not null);
    }

    /// <summary>
    /// Gets the command to select an image.
    /// </summary>
    public ICommand SelectImageCommand { get; }

    /// <summary>
    /// Gets the command to convert the image.
    /// </summary>
    public ICommand ConvertImageCommand { get; }

    /// <summary>
    /// Gets the command to clear selected image.
    /// </summary>
    public ICommand ClearSelectedImageCommand { get; }

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
            CommandManager.InvalidateRequerySuggested();
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
