namespace ImageSquared.ViewModel;

using ImageSquared.Core;
using System.Windows.Input;

public sealed class MainViewModel : ViewModel
{
    private ViewModel? selectedViewModel;

    public MainViewModel(ConversionViewModel conversionViewModel, HistoryViewModel historyViewModel)
    {
        this.ConversionViewModel = Guard.ThrowIfNull(conversionViewModel);
        this.HistoryViewModel = Guard.ThrowIfNull(historyViewModel);
        this.SelectViewModelCommand = new RelayCommand(this.SelectViewModel);

        // Initialize to conversion view when loaded.
        this.SelectedViewModel = conversionViewModel;
    }

    public ICommand SelectViewModelCommand { get; }

    public ConversionViewModel ConversionViewModel { get; }

    public HistoryViewModel HistoryViewModel { get; }

    public ViewModel? SelectedViewModel
    {
        get => this.selectedViewModel;
        set
        {
            this.selectedViewModel = value;
            this.OnPropertyChanged();
        }
    }

    public void SelectViewModel(object? args)
    {
        this.SelectedViewModel = args as ViewModel;
    }
}
