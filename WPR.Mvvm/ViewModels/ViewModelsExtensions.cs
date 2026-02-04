using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reflection;
using WPR.Mvvm.Attributes;

namespace WPR.Mvvm.ViewModels;

internal static class ViewModelsExtensions
{
    /// <summary>
    /// Работаем с атрибутами AutoNotifyCanExecuteChanged.
    /// </summary>
    public static void InitializeAutoNotifyCanExecuteChangedAttribute(this INotifyPropertyChanged viewModel, bool Unregister = false)
    {
        var viewModelType = viewModel.GetType();
        var methods = viewModelType
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(m => m.GetCustomAttribute<AutoNotifyCanExecuteChangedAttribute>() != null);

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<AutoNotifyCanExecuteChangedAttribute>()!;
            string commandName = attr.CommandName ?? InferCommandName(method.Name);

            var commandProp = viewModelType.GetProperty(commandName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (commandProp == null)
                throw new InvalidOperationException($"Команда '{commandName}' не найдена");

            // Принудительно активируем свойство (чтобы сработал ленивый get => ...)
            var commandValue = commandProp.GetValue(viewModel);
            if (commandValue is not IRelayCommand relayCommand)
                throw new InvalidOperationException($"Свойство '{commandName}' не является IRelayCommand");

            if (Unregister)
                GlobalCanExecuteNotifier.Unregister(relayCommand);
            else
            {
                GlobalCanExecuteNotifier.Register(relayCommand);
                relayCommand.NotifyCanExecuteChanged(); // сразу обновляем состояние команды
            }
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