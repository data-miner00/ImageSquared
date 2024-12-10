namespace ImageSquared.View;

using Autofac;
using ImageSquared.ViewModel;
using System.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : Window
{
    public static IComponentContext Context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="settings">The default settings.</param>
    /// <param name="openFileDialog">The open file dialog.</param>
    /// <param name="historyRepository">The history repository.</param>
    /// <param name="outputNaming">The naming strategy for output.</param>
    /// <param name="encoders">The collection of Bitmap encoders.</param>
    public MainWindow(MainViewModel viewModel, IComponentContext context)
    {
        Context = context;

        this.DataContext = viewModel;

        this.InitializeComponent();
    }
}
