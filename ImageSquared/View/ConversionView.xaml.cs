namespace ImageSquared.View;

using Autofac;
using ImageSquared.Core;
using ImageSquared.Core.Models;
using ImageSquared.Core.Repositories;
using ImageSquared.Option;
using ImageSquared.Transformers;
using ImageSquared.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

/// <summary>
/// Interaction logic for ConversionView.xaml.
/// </summary>
public sealed partial class ConversionView : UserControl
{
    private readonly DefaultSettings settings;
    private readonly OpenFileDialog openFileDialog;
    private readonly IHistoryRepository<string> historyRepository;
    private readonly IOutputNamingStrategy outputNaming;
    private readonly IDictionary<ImageFormat, BitmapEncoder> encoders;
    private readonly MainWindowViewModel viewModel;
    private readonly IImageTransformer transformer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConversionView"/> class.
    /// </summary>
    public ConversionView()
    {
        var context = MainWindow.Context;

        this.settings = context.Resolve<DefaultSettings>();
        this.openFileDialog = context.Resolve<OpenFileDialog>();
        this.historyRepository = context.Resolve<IHistoryRepository<string>>();
        this.outputNaming = context.Resolve<IOutputNamingStrategy>();
        this.encoders = context.Resolve<IDictionary<ImageFormat, BitmapEncoder>>();
        this.viewModel = new MainWindowViewModel(this.BrowseImageFile, this.btnConvert_Click);
        this.transformer = context.Resolve<IImageTransformer>();

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

    private void btnConvert_Click()
    {
        if (this.viewModel.ImageOrientation == ImageOrientation.Squared)
        {
            return;
        }

        var currentImage = new Image
        {
            Source = this.viewModel.CurrentImage,
        };

        var transformedImage = this.transformer.Transform(currentImage);

        this.viewModel.TransformedBitmapImage = transformedImage.Source as RenderTargetBitmap;

        this.btnSave_Click();
    }

    private void SaveSelectedFile(string filePath)
    {
        // dont await
        this.historyRepository.AddAsync(filePath);
    }

    private void btnSave_Click()
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
