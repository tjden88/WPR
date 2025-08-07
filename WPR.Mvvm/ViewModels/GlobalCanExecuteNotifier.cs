using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace WPR.Mvvm.ViewModels;

internal static class GlobalCanExecuteNotifier
{
    private static readonly List<IRelayCommand> _Commands = new();

    static GlobalCanExecuteNotifier()
    {
        CommandManager.RequerySuggested += (_, _) =>
        {
            foreach (var cmd in _Commands.ToArray()) // копия на случай удаления
                cmd.NotifyCanExecuteChanged();
        };
    }

    public static void Register(IRelayCommand command)
    {
        if (!_Commands.Contains(command))
            _Commands.Add(command);
    }

    public static void Unregister(IRelayCommand command)
    {
        var removed = _Commands.Remove(command);
        if(!removed)
            Debug.WriteLine($"Команда {command} не была зарегистрирована в GlobalCanExecuteNotifier.");
    }
}