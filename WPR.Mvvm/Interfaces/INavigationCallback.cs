namespace WPR.Mvvm.Interfaces;

/// <summary>
/// Инициирует действия при переходе на это представление
/// </summary>
public interface INavigationCallback
{
    /// <summary>
    /// Вызывается при переходе на это представление с помощью INavigator
    /// </summary>
    void OnNavigated();
}