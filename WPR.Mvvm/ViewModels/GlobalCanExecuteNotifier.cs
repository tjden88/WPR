using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Регистрация команд RelayCommand в Command Manager WPF.
/// Свойства, помеченные атрибутом AutoNotifyCanExecuteChangedAttribute, будут автоматически зарегистрированы  в обработчике
/// </summary>
internal static class GlobalCanExecuteNotifier
{
    private static readonly List<WeakReference<IRelayCommand>> _Commands = new();

    static GlobalCanExecuteNotifier()
    {
        CommandManager.RequerySuggested += (_, _) =>
        {
            // Делаем копию, чтобы безопасно модифицировать
            foreach (var weakRef in _Commands.ToArray())
            {
                if (weakRef.TryGetTarget(out var cmd))
                {
                    cmd.NotifyCanExecuteChanged();
                }
                else
                {
                    // Цель собрана GC — удаляем
                    _Commands.Remove(weakRef);
                    Debug.WriteLine($"Команда {weakRef} была автоматически удалена из GlobalCanExecuteNotifier.");
                }
            }
        };
    }

    public static void Register(IRelayCommand command)
    {
        // Убираем уже собранные ссылки
        Cleanup();
        
        // Не добавляем, если уже есть
        if (_Commands.Any(wr => wr.TryGetTarget(out var cmd) && cmd == command))
            return;

        _Commands.Add(new WeakReference<IRelayCommand>(command));
    }

    public static void Unregister(IRelayCommand command)
    {
        var removed = _Commands.RemoveAll(wr => wr.TryGetTarget(out var cmd) && cmd == command) > 0;
        if (!removed)
            Debug.WriteLine($"Команда {command} не была зарегистрирована в GlobalCanExecuteNotifier.");
    }

    private static void Cleanup()
    {
        var removedCount = _Commands.RemoveAll(wr => !wr.TryGetTarget(out _));
        Debug.WriteLine($"GlobalCanExecuteNotifier cleanup: {removedCount}");
    }
}