namespace ImageSquared.View;

using ImageSquared.Core;
using ImageSquared.Core.Models;
using ImageSquared.Core.Repositories;
using ImageSquared.Option;
using ImageSquared.ViewModel;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : Window
{
    private static readonly Brush BlackBrush = new SolidColorBrush(Colors.Black);
    private static readonly Pen Pen = new(Brushes.Blue, 5); // Blue pen with 5px thickness

    private readonly MainWindowViewModel viewModel;
    private readonly int similarityPercentageThreshold;
    private readonly int dpiX = 96;
    private readonly int dpiY = 96;
    private readonly bool debug;
    private readonly string storageFolder;
    private readonly OpenFileDialog openFileDialog;
    private readonly IDictionary<ImageFormat, BitmapEncoder> encoders;
    private readonly IOutputNamingStrategy outputNaming;
    private readonly IHistoryRepository historyRepository;
    private readonly ImageFormat currentImageFormat;

    static MainWindow()
    {
        Pen.DashStyle = DashStyles.Dash;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="settings">The default settings.</param>
    /// <param name="openFileDialog">The open file dialog.</param>
    /// <param name="historyRepository">The history repository.</param>
    /// <param name="outputNaming">The naming strategy for output.</param>
    /// <param name="encoders">The collection of Bitmap encoders.</param>
    public MainWindow(
        DefaultSettings settings,
        OpenFileDialog openFileDialog,
        IHistoryRepository historyRepository,
        IOutputNamingStrategy outputNaming,
        IDictionary<ImageFormat, BitmapEncoder> encoders,
        MainViewModel viewModel2)
    {
        Guard.ThrowIfNull(settings);

        this.viewModel = new(this.BrowseImageFile);
        this.DataContext = viewModel2;

        this.historyRepository = Guard.ThrowIfNull(historyRepository);
        this.similarityPercentageThreshold = settings.SimilarityPercentageThreshold;
        this.debug = settings.Debug;

        this.storageFolder = settings.StorageFolderPath;
        Directory.CreateDirectory(this.storageFolder);

        this.openFileDialog = openFileDialog;
        this.encoders = Guard.ThrowIfNull(encoders);
        this.outputNaming = Guard.ThrowIfNull(outputNaming);
        this.currentImageFormat = settings.OutputSettings.ImageFormat;

        this.InitializeComponent();
    }

    internal MainWindowViewModel ViewModel => this.viewModel;

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        this.BrowseImageFile();
    }

    private void BrowseImageFile()
    {
        if (this.openFileDialog.ShowDialog() == true)
        {
            var originalImage = new BitmapImage(new Uri(this.openFileDialog.FileName));
            originalImage.Freeze();

            var originalHeight = Convert.ToInt32(originalImage.Height);
            var originalWidth = Convert.ToInt32(originalImage.Width);

            this.viewModel.CurrentImage = originalImage;
            this.viewModel.CurrentImageHeight = originalHeight;
            this.viewModel.CurrentImageWidth = originalWidth;

            this.viewModel.StandardizedLength = this.viewModel.ImageOrientation == ImageOrientation.Landscape
                ? this.viewModel.CurrentImageWidth
                : this.viewModel.CurrentImageHeight;

            this.SaveSelectedFile(this.openFileDialog.FileName);
        }
    }

    private void btnConvert_Click(object sender, RoutedEventArgs e)
    {
        if (this.viewModel.ImageOrientation == ImageOrientation.Squared)
        {
            return;
        }

        var imageOrientation = this.viewModel.ImageOrientation;
        var standardLength = this.viewModel.StandardizedLength;
        var currentImageWidth = this.viewModel.CurrentImageWidth;
        var currentImageHeight = this.viewModel.CurrentImageHeight;
        var currentImage = this.viewModel.CurrentImage;

        var extendedImage = new RenderTargetBitmap(standardLength, standardLength, this.dpiX, this.dpiY, PixelFormats.Pbgra32);

        var visual = new DrawingVisual();

        using (var drawingContext = visual.RenderOpen())
        {
            if (this.debug)
            {
                drawingContext.DrawRectangle(BlackBrush, Pen, new Rect(0, 0, standardLength, standardLength));
            }

            if (imageOrientation == ImageOrientation.Portrait)
            {
                var startingPosition = (standardLength / 2) - (currentImageWidth / 2);
                drawingContext.DrawImage(currentImage, new Rect(startingPosition, 0, currentImageWidth, currentImageHeight));
            }
            else if (imageOrientation == ImageOrientation.Landscape)
            {
                var startingPosition = (standardLength / 2) - (currentImageHeight / 2);
                drawingContext.DrawImage(currentImage, new Rect(0, startingPosition, currentImageWidth, currentImageHeight));
            }
            else
            {
                drawingContext.DrawImage(currentImage, new Rect(0, 0, currentImageWidth, currentImageHeight));
            }
        }

        extendedImage.Render(visual);

        this.viewModel.TransformedBitmapImage = extendedImage;

        this.btnSave_Click(sender, e);
    }

    private void SaveSelectedFile(string filePath)
    {
        // dont await
        this.historyRepository.AddAsync(filePath);
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (this.viewModel.TransformedBitmapImage is null)
        {
            _ = MessageBox.Show("Please select an image first", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var randomName = this.outputNaming.Generate();

        var fullPath = System.IO.Path.Combine(this.storageFolder, randomName);

        this.encoders.TryGetValue(this.currentImageFormat, out var encoder);

        if (encoder is null)
        {
            throw new InvalidOperationException("Image format not supported.");
        }

        using (var fileStream = File.Open(fullPath, FileMode.Create))
        {
            encoder.Frames.Add(BitmapFrame.Create(this.viewModel.TransformedBitmapImage));

            encoder.Save(fileStream);
        }

        _ = MessageBox.Show("Successfully converted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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
}
