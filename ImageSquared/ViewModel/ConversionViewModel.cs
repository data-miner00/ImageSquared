namespace ImageSquared.ViewModel;

using ImageSquared.Core;
using ImageSquared.Core.Models;
using ImageSquared.Core.Repositories;
using ImageSquared.Option;
using ImageSquared.Transformers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

/// <summary>
/// The view model for conversion page.
/// </summary>
public sealed class ConversionViewModel : ViewModel
{
    private readonly DefaultSettings defaultSettings;
    private readonly OpenFileDialog openFileDialog;
    private readonly IHistoryRepository<LoadHistoryRecord> historyRepository;
    private readonly IOutputNamingStrategy outputNamingStrategy;
    private readonly IDictionary<ImageFormat, BitmapEncoder> encoders;
    private readonly IImageTransformer transformer;

    private int currentImageWidth;
    private int currentImageHeight;
    private int standardizedLength;
    private BitmapImage? currentImage;
    private RenderTargetBitmap? transformedBitmapImage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConversionViewModel"/> class.
    /// </summary>
    /// <param name="defaultSettings">The default settings.</param>
    /// <param name="openFileDialog">The open file dialog.</param>
    /// <param name="historyRepository">The history repository.</param>
    /// <param name="outputNamingStrategy">The naming strategy.</param>
    /// <param name="encoders">The image encoders.</param>
    /// <param name="transformer">The image transformer.</param>
    public ConversionViewModel(
        DefaultSettings defaultSettings,
        OpenFileDialog openFileDialog,
        IHistoryRepository<LoadHistoryRecord> historyRepository,
        IOutputNamingStrategy outputNamingStrategy,
        IDictionary<ImageFormat, BitmapEncoder> encoders,
        IImageTransformer transformer)
    {
        this.defaultSettings = defaultSettings;
        this.openFileDialog = openFileDialog;
        this.historyRepository = historyRepository;
        this.outputNamingStrategy = outputNamingStrategy;
        this.encoders = encoders;
        this.transformer = transformer;

        // Commands
        this.SelectImageCommand = new RelayCommand(_ => this.BrowseImageFile());
        this.ConvertImageCommand = new RelayCommand(_ => this.ConvertImage(), (_) => this.currentImage is not null);
        this.ClearSelectedImageCommand = new RelayCommand(_ => this.Reset(), (_) => this.currentImage is not null);
    }

    #region Commands

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

    #endregion

    #region Getters and Setters

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

    #endregion

    #region Methods

    private void BrowseImageFile()
    {
        if (this.openFileDialog.ShowDialog() != true)
        {
            return;
        }

        var openFileName = this.openFileDialog.FileName;

        var originalImage = new BitmapImage(new Uri(openFileName));
        originalImage.Freeze();

        var originalHeight = Convert.ToInt32(originalImage.Height);
        var originalWidth = Convert.ToInt32(originalImage.Width);

        this.CurrentImage = originalImage;
        this.CurrentImageHeight = originalHeight;
        this.CurrentImageWidth = originalWidth;

        this.StandardizedLength = this.ImageOrientation == ImageOrientation.Landscape
            ? this.CurrentImageWidth
            : this.CurrentImageHeight;

        this.historyRepository.AddAsync(openFileName);
    }

    private void ConvertImage()
    {
        if (this.ImageOrientation == ImageOrientation.Squared)
        {
            return;
        }

        if (this.CurrentImage is null)
        {
            MessageBox.Show("Please select an image first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var currentImageObject = new Image
        {
            Source = this.CurrentImage,
        };

        var transformedImage = this.transformer.Transform(currentImageObject);

        this.TransformedBitmapImage = transformedImage.Source as RenderTargetBitmap;

        var randomName = this.outputNamingStrategy.Generate();

        var fullPath = System.IO.Path.Combine(this.defaultSettings.StorageFolderPath, randomName);

        if (!this.encoders.TryGetValue(this.defaultSettings.OutputSettings.ImageFormat, out var encoder))
        {
            MessageBox.Show("Image format not supported", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        using (var fileStream = File.Open(fullPath, FileMode.Create))
        {
            encoder.Frames.Add(BitmapFrame.Create(this.TransformedBitmapImage));
            encoder.Save(fileStream);
        }

        MessageBox.Show("Successfully converted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        var startProcessInfo = new ProcessStartInfo
        {
            FileName = fullPath,
            UseShellExecute = true,
        };

        try
        {
            Process.Start(startProcessInfo);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    private void Reset()
    {
        this.CurrentImage = null;
        this.CurrentImageWidth = 0;
        this.CurrentImageHeight = 0;
        this.StandardizedLength = 0;
    }

    #endregion
}
