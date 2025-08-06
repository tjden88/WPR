using System.Windows;

namespace WPR.Mvvm.Interfaces;

/// <summary>
/// Регистратор представлений и моделей-представлений
/// Автоматически отображает модель-представление в соответствии со связанным представлением
/// </summary>
public interface IViewsRegistrator
{
    /// <summary>
    /// Добавить представление и связанную модель-представление
    /// </summary>
    /// <typeparam name="TView">Тип представления</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления)</param>
    IViewsRegistrator AddSingleton<TView, TViewModel>(string? Name = null) where TView : FrameworkElement, new();


    /// <summary>
    /// Добавить представление и найти связанную модель-представление
    /// Поиск происходит в той же сборке, ищется суффикс ViewModel
    /// </summary>
    /// <typeparam name="TView">Тип представления</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления)</param>
    IViewsRegistrator AddSingleton<TView>(string? Name = null) where TView : FrameworkElement, new();



    /// <summary>
    /// Добавить представление и связанную модель-представление
    /// </summary>
    /// <typeparam name="TView">Тип представления</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления)</param>
    IViewsRegistrator AddTransient<TView, TViewModel>(string? Name = null) where TView : FrameworkElement, new();


    /// <summary>
    /// Добавить представление и найти связанную модель-представление
    /// Поиск происходит в той же сборке, ищется суффикс ViewModel
    /// </summary>
    /// <typeparam name="TView">Тип представления</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления)</param>
    IViewsRegistrator AddTransient<TView>(string? Name = null)where TView : FrameworkElement, new();

    /// <summary>
    /// Добавить представление и связанную модель-представление
    /// </summary>
    /// <typeparam name="TView">Тип представления</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления)</param>
   IViewsRegistrator AddScoped<TView, TViewModel>(string? Name = null)where TView : FrameworkElement, new();

    /// <summary>
    /// Добавить представление и найти связанную модель-представление
    /// Поиск происходит в той же сборке, ищется суффикс ViewModel
    /// </summary>
    /// <typeparam name="TView">Тип представления</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления)</param>
    IViewsRegistrator AddScoped<TView>(string? Name = null)where TView : FrameworkElement, new();

    /// <summary>
    /// Зарегистрировать окно со связанной моделью-представления
    /// Если модель-представление IDisposable - она уничтожается при закрытии окна
    /// </summary>
    /// <typeparam name="TWindow">Тип представления</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления)</param>
    IViewsRegistrator AddTransientWindow<TWindow, TViewModel>(string? Name = null) where TWindow : Window, new();

    /// <summary>
    /// Добавить главное окно со связанной моделью-представления (Singleton).
    /// При закрытии окна приложение завершает работу.
    /// </summary>
    /// <typeparam name="TWindow">Тип представления</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления</typeparam>
    /// <param name="Name">Заданное имя (по умолчанию - имя модели-представления)</param>
    IViewsRegistrator AddMainWindow<TWindow, TViewModel>(string? Name = null) where TWindow : Window, new();


    /// <summary>
    /// Создать только шаблон данных. Не регистрировать модель-представление в коллекции сервисов
    /// Поиск происходит в той же сборке, ищется суффикс ViewModel
    /// </summary>
    /// <typeparam name="TView">Тип представления</typeparam>
    /// <returns></returns>
    IViewsRegistrator AddTemplateOnly<TView>() where TView : FrameworkElement, new();

    /// <summary>
    /// Создать только шаблон данных. Не регистрировать модель-представление в коллекции сервисов
    /// </summary>
    /// <typeparam name="TView">Тип представления</typeparam>
    /// <typeparam name="TViewModel">Тип модели-представления</typeparam>
    /// <returns></returns>
    IViewsRegistrator AddTemplateOnly<TView, TViewModel>()where TView : FrameworkElement, new();
}