namespace ImageSquared.ViewModel;

using System.Windows.Input;

public sealed class MainViewModel : ViewModel
{
    private ViewModel? selectedViewModel;

    public MainViewModel(ConversionViewModel conversionViewModel, HistoryViewModel historyViewModel)
    {
        this.ConversionViewModel = conversionViewModel;
        this.HistoryViewModel = historyViewModel;
        this.SelectViewModelCommand = new RelayCommand(this.SelectViewModel);
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
