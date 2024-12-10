namespace ImageSquared.View;

using Autofac;
using ImageSquared.Core;
using ImageSquared.Core.Models;
using ImageSquared.Core.Repositories;
using ImageSquared.Option;
using ImageSquared.ViewModel;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/// <summary>
/// Interaction logic for ConversionView.xaml
/// </summary>
public sealed partial class ConversionView : UserControl
{
    private readonly DefaultSettings settings;
    private readonly OpenFileDialog openFileDialog;
    private readonly IHistoryRepository historyRepository;
    private readonly IOutputNamingStrategy outputNaming;
    private readonly IDictionary<ImageFormat, BitmapEncoder> encoders;
    private readonly MainWindowViewModel viewModel;

    private readonly int dpiX = 96;
    private readonly int dpiY = 96;

    public ConversionView()
    {
        var context = MainWindow.Context;

        this.settings = context.Resolve<DefaultSettings>();
        this.openFileDialog = context.Resolve<OpenFileDialog>();
        this.historyRepository = context.Resolve<IHistoryRepository>();
        this.outputNaming = context.Resolve<IOutputNamingStrategy>();
        this.encoders = context.Resolve<IDictionary<ImageFormat, BitmapEncoder>>();
        this.viewModel = new MainWindowViewModel(this.BrowseImageFile);

        this.DataContext = this.viewModel;

        this.InitializeComponent();
    }

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

        var fullPath = System.IO.Path.Combine(this.settings.StorageFolderPath, randomName);

        this.encoders.TryGetValue(this.settings.OutputSettings.ImageFormat, out var encoder);

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
