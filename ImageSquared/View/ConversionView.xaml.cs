namespace ImageSquared.View;

using Autofac;
using ImageSquared.ViewModel;
using System.Windows.Controls;

/// <summary>
/// Interaction logic for ConversionView.xaml.
/// </summary>
public sealed partial class ConversionView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConversionView"/> class.
    /// </summary>
    public ConversionView()
    {
        var context = MainWindow.Context;
        var viewModel = context.Resolve<ConversionViewModel>();
        this.DataContext = viewModel;
        this.InitializeComponent();
    }
}
