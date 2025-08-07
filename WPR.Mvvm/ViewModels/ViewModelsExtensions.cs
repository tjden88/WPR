using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace WPR.Mvvm.ViewModels;

internal static class ViewModelsExtensions
{
    public static void InitializeAttributes(this INotifyPropertyChanged viewModel)
    {
        var viewModelType = viewModel.GetType();
        var methods = viewModelType
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(m => m.GetCustomAttribute<AutoNotifyCanExecuteChangedAttribute>() != null);

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<AutoNotifyCanExecuteChangedAttribute>()!;

            // Определяем имя команды
            // Если атрибут пустой, пытаемся догадаться по имени метода
            string commandName = attr.CommandName ?? InferCommandName(method.Name);

            var commandProp = viewModelType.GetProperty(commandName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (commandProp == null)
            {
                throw new InvalidOperationException($"Команда с именем '{commandName}' не найдена в {viewModelType.Name}");
            }

            if (commandProp.GetValue(viewModel) is not IRelayCommand relayCommand)
            {
                throw new InvalidOperationException($"Свойство '{commandName}' не реализует IRelayCommand");
            }

            // Подписываемся на CommandManager.RequerySuggested через WeakEventManager
            WeakEventManager<CommandManager, EventArgs>.AddHandler(null!, nameof(CommandManager.RequerySuggested), (_, _) =>
            {
                relayCommand.NotifyCanExecuteChanged();
            });
        }
    }

    /// <summary>
    /// Попытка вывести имя команды из имени метода SaveAsync -> SaveCommand, Save -> SaveCommand
    /// </summary>
    private static string InferCommandName(string methodName)
    {
        if (methodName.EndsWith("Async"))
            methodName = methodName[..^5]; // отрезать Async

        return methodName + "Command";
    }
}