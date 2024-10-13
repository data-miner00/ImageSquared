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

namespace ImageSquared
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Brush BlackBrush = new SolidColorBrush(Colors.Black);
        private static readonly Pen Pen = new Pen(Brushes.Blue, 5); // Blue pen with 5px thickness

        private readonly int similarityPercentageThreshold = 10;
        private readonly int dpiX = 96;
        private readonly int dpiY = 96;
        private readonly bool debug = false;

        static MainWindow()
        {
            Pen.DashStyle = DashStyles.Dash; 
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var originalImage = new BitmapImage(new Uri(openFileDialog.FileName));
                this.SelectedImage.Source = originalImage;
                originalImage.Freeze();

                var originalHeight = Convert.ToInt32(this.SelectedImage.Height);
                var originalWidth = Convert.ToInt32(this.SelectedImage.Width);

                if (originalHeight == originalWidth)
                {
                    return;
                }

                var imageOrientation = (originalHeight - originalWidth) > 0
                    ? ImageOrientation.Portrait
                    : ImageOrientation.Landscape;

                var standardLength = imageOrientation == ImageOrientation.Landscape
                    ? originalWidth
                    : originalHeight;

                var extendedImage = new RenderTargetBitmap(standardLength, standardLength, this.dpiX, this.dpiY, PixelFormats.Pbgra32);

                var visual = new DrawingVisual();

                using (var drawingContext = visual.RenderOpen())
                {
                    if (this.debug)
                    {
                        drawingContext.DrawRectangle(BlackBrush, Pen, new Rect(0, 0, standardLength, standardLength));
                    }

                    drawingContext.DrawImage(originalImage, new Rect(0, 0, originalWidth, originalHeight));
                }

                extendedImage.Render(visual);

                this.TransformedImage.Source = extendedImage;
            }
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