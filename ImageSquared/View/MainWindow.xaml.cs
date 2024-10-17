using ImageSquared.Core;
using ImageSquared.Option;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageSquared.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Brush BlackBrush = new SolidColorBrush(Colors.Black);
        private static readonly Pen Pen = new Pen(Brushes.Blue, 5); // Blue pen with 5px thickness

        private readonly int similarityPercentageThreshold;
        private readonly int dpiX = 96;
        private readonly int dpiY = 96;
        private readonly bool debug;
        private readonly string filter;

        private BitmapImage currentImage;
        private int currentImageHeight;
        private int currentImageWidth;

        static MainWindow()
        {
            Pen.DashStyle = DashStyles.Dash;
        }

        public MainWindow(DefaultSettings settings)
        {
            Guard.ThrowIfNull(settings);

            this.similarityPercentageThreshold = settings.SimilarityPercentageThreshold;
            this.debug = settings.Debug;
            this.filter = settings.OpenFileDialogFilter;

            // var result = MessageBox.Show("Started", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = this.filter,
                Title = "Select an image file",
                Multiselect = false,
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var originalImage = new BitmapImage(new Uri(openFileDialog.FileName));
                this.SelectedImage.Source = originalImage;
                originalImage.Freeze();

                var originalHeight = Convert.ToInt32(originalImage.Height);
                var originalWidth = Convert.ToInt32(originalImage.Width);

                this.currentImage = originalImage;
                this.currentImageHeight = originalHeight;
                this.currentImageWidth = originalWidth;
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

                drawingContext.DrawImage(this.currentImage, new Rect(0, 0, this.currentImageWidth, this.currentImageHeight));
            }

            extendedImage.Render(visual);

            // this.TransformedImage.Source = extendedImage;
        }
    }

    public enum ImageOrientation
    {
        None,

        Portrait,

        Landscape,

        Squared,
    }
}
