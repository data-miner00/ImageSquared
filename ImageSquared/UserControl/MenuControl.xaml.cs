namespace ImageSquared.UserControl;

using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Interaction logic for MenuControl.xaml.
/// </summary>
public partial class MenuControl : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MenuControl"/> class.
    /// </summary>
    public MenuControl()
    {
        this.InitializeComponent();
    }

    private void menuExit_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
