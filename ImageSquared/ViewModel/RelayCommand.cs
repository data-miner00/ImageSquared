namespace ImageSquared.ViewModel;

using ImageSquared.Core;
using System;
using System.Windows.Input;

/// <summary>
/// Simple implementation for <see cref="ICommand"/>.
/// </summary>
internal sealed class RelayCommand : ICommand
{
    private readonly Action<object?> execute;
    private readonly Func<object?, bool>? canExecute;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">The predicate for execution allowed.</param>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        this.execute = Guard.ThrowIfNull(execute);
        this.canExecute = canExecute;
    }

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <inheritdoc/>
    public bool CanExecute(object? parameter)
    {
        return this.canExecute == null || this.canExecute(parameter);
    }

    /// <inheritdoc/>
    public void Execute(object? parameter)
    {
        this.execute(parameter);
    }
}
