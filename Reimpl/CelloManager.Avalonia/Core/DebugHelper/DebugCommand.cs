using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace CelloManager.Core.DebugHelper;

[PublicAPI]
public class DebugCommand(ICommand command) : ICommand
{
    public bool CanExecute(object? parameter) => command.CanExecute(parameter);

    public void Execute(object? parameter)
    {
        command.Execute(parameter);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => command.CanExecuteChanged += value;
        remove => command.CanExecuteChanged -= value;
    }

    public static ICommand Wrap(ICommand command)
        => new DebugCommand(command);
}