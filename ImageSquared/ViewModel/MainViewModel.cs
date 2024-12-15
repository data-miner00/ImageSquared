namespace ImageSquared.ViewModel;

using ImageSquared.Core;
using ImageSquared.View;
using System.Windows.Input;

/// <summary>
/// The main view model. Determines which child view model to use.
/// </summary>
public sealed class MainViewModel : ViewModel
{
    private ViewModel? selectedViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="conversionViewModel">The view model for <see cref="ConversionView"/>.</param>
    /// <param name="historyViewModel">The view model for <see cref="HistoryView"/>.</param>
    public MainViewModel(ConversionViewModel conversionViewModel, HistoryViewModel historyViewModel)
    {
        this.ConversionViewModel = Guard.ThrowIfNull(conversionViewModel);
        this.HistoryViewModel = Guard.ThrowIfNull(historyViewModel);
        this.SelectViewModelCommand = new RelayCommand(this.SelectViewModel);

        // Initialize to conversion view when loaded.
        this.SelectedViewModel = conversionViewModel;
    }

    /// <summary>
    /// Gets the select view model command.
    /// </summary>
    public ICommand SelectViewModelCommand { get; }

    /// <summary>
    /// Gets the conversion view model.
    /// </summary>
    public ConversionViewModel ConversionViewModel { get; }

    /// <summary>
    /// Gets the history view model.
    /// </summary>
    public HistoryViewModel HistoryViewModel { get; }

    /// <summary>
    /// Gets or sets the current selected view model.
    /// </summary>
    public ViewModel? SelectedViewModel
    {
        get => this.selectedViewModel;
        set
        {
            this.selectedViewModel = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Changes the view model from the argument.
    /// </summary>
    /// <param name="args">The selected view model.</param>
    public void SelectViewModel(object? args)
    {
        this.SelectedViewModel = args as ViewModel;
    }
}
