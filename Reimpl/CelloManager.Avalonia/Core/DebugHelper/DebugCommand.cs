using System;
using System.Windows.Input;

namespace CelloManager.Avalonia.Core.DebugHelper;

public class DebugCommand : ICommand
{
    public static ICommand Wrap(ICommand command)
        => new DebugCommand(command);
    
    private readonly ICommand _command;

    public DebugCommand(ICommand command) => _command = command;

    public bool CanExecute(object? parameter)
    {
        return _command.CanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _command.Execute(parameter);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => _command.CanExecuteChanged += value;
        remove => _command.CanExecuteChanged -= value;
    }
}