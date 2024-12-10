namespace ImageSquared.View;

using ImageSquared.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

/// <summary>
/// Interaction logic for HistoryWindow.xaml
/// </summary>
public partial class HistoryWindow : Window
{
    private readonly IHistoryRepository repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryWindow"/> class.
    /// </summary>
    /// <param name="repository">The history repository.</param>
    public HistoryWindow(IHistoryRepository repository)
    {
        this.repository = repository;
        this.DataContext = this;

        this.LoadHistoryFile();

        this.InitializeComponent();
    }

    /// <summary>
    /// Gets the observable file history.
    /// </summary>
    public ObservableCollection<string> FileHistory { get; } = [];

    private void LoadHistoryFile()
    {
        var fileNames = this.repository.GetAllAsync().GetAwaiter().GetResult();

        foreach (var filePath in fileNames)
        {
            this.FileHistory.Add(filePath);
        }
    }
}
