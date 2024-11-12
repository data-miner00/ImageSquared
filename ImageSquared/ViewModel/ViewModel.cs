namespace ImageSquared.ViewModel;

using System.ComponentModel;
using System.Runtime.CompilerServices;

internal abstract class ViewModel : INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notify changes from a property.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        var args = new PropertyChangedEventArgs(propertyName);
        this.PropertyChanged?.Invoke(this, args);
    }
}
