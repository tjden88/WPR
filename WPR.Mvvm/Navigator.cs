using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WPR.Mvvm.Interfaces;

namespace WPR.Mvvm;

/// <summary> Навигация в приложении </summary>
public static class Navigator
{
    public class NavigationEventArgs(string Region, string ViewModelName) : EventArgs
    {
        public string ViewModelName { get; init; } = ViewModelName;

        public string Region { get; init; } = Region;
    }

    /// <summary>
    /// Происходит при успешной навигации
    /// </summary>
    public static event EventHandler<NavigationEventArgs>? Navigated;

    /// <summary>
    /// Установить контент для выбранной области
    /// </summary>
    /// <param name="Region">Область навигации</param>
    /// <param name="ViewModelName">Имя зарегистрированной вьюмодели</param>
    public static void Navigate(string Region, string ViewModelName)
    {
        var hasRegion = Mvvm.AttachedRegions.TryGetValue(Region, out var value);
        if (!hasRegion)
            throw new ArgumentNullException(nameof(Region), "Область не назначена");

        if (SetContent(value!, ViewModelName))
            Navigated?.Invoke(null, new NavigationEventArgs(Region, ViewModelName));
    }

    /// <summary>
    /// Установить контент для выбранной области
    /// </summary>
    /// <param name="Region">Область навигации</param>
    /// <param name="ViewModel">Экземпляр объекта, который будет установлен в качестве контента</param>
    public static void Navigate(string Region, object ViewModel)
    {
        var hasRegion = Mvvm.AttachedRegions.TryGetValue(Region, out var value);
        if (!hasRegion)
            throw new ArgumentNullException(nameof(Region), "Область не назначена");

        value!.Content = ViewModel;
        if (ViewModel is INavigationCallback navigationCallback) 
            navigationCallback.OnNavigated();

        Navigated?.Invoke(null, new NavigationEventArgs(Region, nameof(ViewModel)));
    }
    


    /// <summary> Установить контент в контент контрол </summary>
    internal static bool SetContent(ContentControl Control, string ViewModelName)
    {
        if (DesignerProperties.GetIsInDesignMode(Control))
            return false;

        var hasType = Mvvm.RegisteredViews.TryGetValue(ViewModelName, out var viewModelType);
        if (!hasType)
            throw new ArgumentNullException(nameof(ViewModelName),
                $"Модель-представление с именем {ViewModelName} не была зарегистрирована");

        var instance = ServiceResolver.Services.GetRequiredService(viewModelType!);

        if (Equals(Control.Content, instance))
            return false;

        Control.Content = instance;
        if (instance is INavigationCallback navigationCallback) navigationCallback.OnNavigated();

        return true;
    }
}