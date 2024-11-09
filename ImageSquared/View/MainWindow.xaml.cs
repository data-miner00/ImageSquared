﻿namespace ImageSquared.View;

using ImageSquared.Core;
using ImageSquared.Core.Repositories;
using ImageSquared.Option;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using IO = System.IO;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : Window
{
    private static readonly Brush BlackBrush = new SolidColorBrush(Colors.Black);
    private static readonly Pen Pen = new(Brushes.Blue, 5); // Blue pen with 5px thickness

    private readonly int similarityPercentageThreshold;
    private readonly int dpiX = 96;
    private readonly int dpiY = 96;
    private readonly bool debug;
    private readonly string storageFolder;
    private readonly OpenFileDialog openFileDialog;
    private readonly Func<HistoryWindow> historyWindow;
    private readonly IHistoryRepository historyRepository;
    private BitmapImage? currentImage;
    private RenderTargetBitmap? transformedBitmapImage;
    private int currentImageHeight;
    private int currentImageWidth;

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
    /// <param name="historyWindow">The history window.</param>
    public MainWindow(
        DefaultSettings settings,
        OpenFileDialog openFileDialog,
        IHistoryRepository historyRepository,
        Func<HistoryWindow> historyWindow)
    {
        Guard.ThrowIfNull(settings);
        this.DataContext = this;

        this.historyRepository = Guard.ThrowIfNull(historyRepository);
        this.similarityPercentageThreshold = settings.SimilarityPercentageThreshold;
        this.debug = settings.Debug;
        this.storageFolder = settings.StorageFolderPath;
        this.openFileDialog = openFileDialog;
        this.historyWindow = historyWindow;
        this.InitializeComponent();
    }

    private static string GenerateRandomImageName()
    {
        var randomName = $"{Guid.NewGuid()}.png";
        return randomName;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (this.openFileDialog.ShowDialog() == true)
        {
            var originalImage = new BitmapImage(new Uri(this.openFileDialog.FileName));
            this.SelectedImage.Source = originalImage;
            originalImage.Freeze();

            var originalHeight = Convert.ToInt32(originalImage.Height);
            var originalWidth = Convert.ToInt32(originalImage.Width);

            this.currentImage = originalImage;
            this.currentImageHeight = originalHeight;
            this.currentImageWidth = originalWidth;

            this.SaveSelectedFile(this.openFileDialog.FileName);
        }
    }

    private void btnConvert_Click(object sender, RoutedEventArgs e)
    {
        if (this.currentImageHeight == this.currentImageWidth)
        {
            return;
        }

        var imageOrientation = (this.currentImageHeight - this.currentImageWidth) > 0
            ? ImageOrientation.Portrait
            : ImageOrientation.Landscape;

        var standardLength = imageOrientation == ImageOrientation.Landscape
            ? this.currentImageWidth
            : this.currentImageHeight;

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
                var startingPosition = (standardLength / 2) - (this.currentImageWidth / 2);
                drawingContext.DrawImage(this.currentImage, new Rect(startingPosition, 0, this.currentImageWidth, this.currentImageHeight));
            }
            else if (imageOrientation == ImageOrientation.Landscape)
            {
                var startingPosition = (standardLength / 2) - (this.currentImageHeight / 2);
                drawingContext.DrawImage(this.currentImage, new Rect(0, startingPosition, this.currentImageWidth, this.currentImageHeight));
            }
            else
            {
                drawingContext.DrawImage(this.currentImage, new Rect(0, 0, this.currentImageWidth, this.currentImageHeight));
            }
        }

        extendedImage.Render(visual);

        this.transformedBitmapImage = extendedImage;

        this.TransformedImage.Source = extendedImage;
    }

    private void SaveSelectedFile(string filePath)
    {
        // dont await
        this.historyRepository.AddAsync(filePath);
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (this.transformedBitmapImage is null)
        {
            _ = MessageBox.Show("Please select an image first", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var randomName = GenerateRandomImageName();
        Directory.CreateDirectory(this.storageFolder);

        var fullPath = IO.Path.Combine(this.storageFolder, randomName);

        using (var fileStream = File.Open(fullPath, FileMode.Create))
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(this.transformedBitmapImage));

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

    private void btnTogglePosition_Click(object sender, RoutedEventArgs e)
    {
        // same as Grid.GetColumn, Grid.SetColumn
        //var currentPosition = (int)this.sidebar.GetValue(Grid.ColumnProperty);
        //var newColumn = currentPosition == 0 ? 3 : 0;
        //this.sidebar.SetValue(Grid.ColumnProperty, newColumn);

        this.historyWindow().Show();
    }
}
