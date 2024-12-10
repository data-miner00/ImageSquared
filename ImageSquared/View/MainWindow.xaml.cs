namespace ImageSquared.View;

using Autofac;
using ImageSquared.ViewModel;
using System.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public sealed partial class MainWindow : Window
{
    public static IComponentContext Context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The main view model.</param>
    /// <param name="context">The IoC container context.</param>
    public MainWindow(MainViewModel viewModel, IComponentContext context)
    {
        Context ??= context;

        this.DataContext = viewModel;

        this.InitializeComponent();
    }
}
