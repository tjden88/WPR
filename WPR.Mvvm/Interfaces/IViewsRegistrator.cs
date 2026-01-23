using System.Windows;

namespace WPR.Mvvm.Interfaces;

/// <summary>
/// Регистратор представлений и моделей-представлений.
/// Автоматически связывает ViewModel с View и добавляет DataTemplate (ViewModel -> View).
/// </summary>
/// <remarks>
/// ВАЖНО: представления создаются через DI (ActivatorUtilities), поэтому конструктор по умолчанию не требуется.
/// </remarks>
public interface IViewsRegistrator
{
    /// <summary>
    /// Добавить представление и связанную модель-представление (Singleton).
    /// </summary>
    /// <typeparam name="TView">Тип представления.</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления.</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления).</param>
    IViewsRegistrator AddSingleton<TView, TViewModel>(string? Name = null)
        where TView : FrameworkElement;

    /// <summary>
    /// Добавить представление и найти связанную модель-представление (Singleton).
    /// Поиск происходит в той же сборке, ищется суффикс ViewModel по имени типа представления.
    /// </summary>
    /// <typeparam name="TView">Тип представления.</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления).</param>
    IViewsRegistrator AddSingleton<TView>(string? Name = null)
        where TView : FrameworkElement;

    /// <summary>
    /// Добавить представление и связанную модель-представление (Transient).
    /// </summary>
    /// <typeparam name="TView">Тип представления.</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления.</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления).</param>
    IViewsRegistrator AddTransient<TView, TViewModel>(string? Name = null)
        where TView : FrameworkElement;

    /// <summary>
    /// Добавить представление и найти связанную модель-представление (Transient).
    /// Поиск происходит в той же сборке, ищется суффикс ViewModel по имени типа представления.
    /// </summary>
    /// <typeparam name="TView">Тип представления.</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления).</param>
    IViewsRegistrator AddTransient<TView>(string? Name = null)
        where TView : FrameworkElement;

    /// <summary>
    /// Добавить представление и связанную модель-представление (Scoped).
    /// </summary>
    /// <typeparam name="TView">Тип представления.</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления.</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления).</param>
    IViewsRegistrator AddScoped<TView, TViewModel>(string? Name = null)
        where TView : FrameworkElement;

    /// <summary>
    /// Добавить представление и найти связанную модель-представление (Scoped).
    /// Поиск происходит в той же сборке, ищется суффикс ViewModel по имени типа представления.
    /// </summary>
    /// <typeparam name="TView">Тип представления.</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления).</param>
    IViewsRegistrator AddScoped<TView>(string? Name = null)
        where TView : FrameworkElement;

    /// <summary>
    /// Зарегистрировать окно со связанной моделью-представления (Transient).
    /// Если модель-представление <see cref="IDisposable"/> - она уничтожается при закрытии окна.
    /// </summary>
    /// <typeparam name="TWindow">Тип окна.</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления.</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления).</param>
    IViewsRegistrator AddTransientWindow<TWindow, TViewModel>(string? Name = null)
        where TWindow : Window;

    /// <summary>
    /// Добавить главное окно со связанной моделью-представления (Singleton).
    /// При закрытии окна приложение завершает работу.
    /// </summary>
    /// <typeparam name="TWindow">Тип окна.</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления.</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления).</param>
    IViewsRegistrator AddMainWindow<TWindow, TViewModel>(string? Name = null)
        where TWindow : Window;

    /// <summary>
    /// Создать только шаблон данных (ViewModel -> View). Не регистрировать ViewModel в коллекции сервисов.
    /// Поиск ViewModel происходит в той же сборке, ищется суффикс ViewModel по имени типа представления.
    /// </summary>
    /// <typeparam name="TView">Тип представления.</typeparam>
    IViewsRegistrator AddTemplateOnly<TView>()
        where TView : FrameworkElement;

    /// <summary>
    /// Создать только шаблон данных (ViewModel -> View). Не регистрировать ViewModel в коллекции сервисов.
    /// </summary>
    /// <typeparam name="TView">Тип представления.</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления.</typeparam>
    IViewsRegistrator AddTemplateOnly<TView, TViewModel>()
        where TView : FrameworkElement;
}
