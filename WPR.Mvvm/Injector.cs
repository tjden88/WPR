using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WPR.Mvvm;

/// <summary>
/// Инструмент для внедрения зарегистрированных представлений в XAML с привязанной ViewModel.
/// </summary>
public class Injector : Control
{
    static Injector()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Injector), new FrameworkPropertyMetadata(typeof(Injector)));
    }

    #region Inject : Type - Тип представления для отображения

    /// <summary>Тип представления для отображения</summary>
    public static readonly DependencyProperty InjectProperty =
        DependencyProperty.Register(
            nameof(Inject),
            typeof(Type),
            typeof(Injector),
            new PropertyMetadata(null, PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        var injector = (Injector)D;
        if (E.OldValue is IDisposable disposable)
            disposable.Dispose(); // Освобождаем старый объект, если он IDisposable

        if (E.OldValue is FrameworkElement {DataContext: IDisposable dc}) 
            dc.Dispose(); // Освобождаем старую vm, если она IDisposable

        if (E.NewValue is null)
        {
            injector.Content = null; // Если тип не задан, очищаем контент
            return;
        }

        if(E.NewValue is not Type t)
            throw new InvalidOperationException($"Тип должен быть указан в свойстве {nameof(Inject)}.");

        var type = t;
        if (!ServiceResolver.IsInitialized) // Работаем в дизайнере
        {
            var content = Activator.CreateInstance(type);
            injector.Content = content ?? throw new InvalidOperationException($"Не удалось создать экземпляр объекта {type.FullName} в дизайнере. Проверьте, что конструктор не требует параметров или все параметры доступны в дизайнере.");
            return;
        }

        var service = ServiceResolver.Services.GetService(type);

        injector.Content = service ?? throw new InvalidOperationException($"Не удалось найти сервис {type.FullName} в коллекции сервисов. Проверьте регистрацию сервиса в ServiceCollection.");
    }

    /// <summary>Тип представления для отображения</summary>
    [Category("Injector")]
    [Description("Тип представления для отображения")]
    public Type Inject
    {
        get => (Type)GetValue(InjectProperty);
        set => SetValue(InjectProperty, value);
    }

    #endregion

    #region Content : object - Отображение внедрённого типа

    /// <summary>Отображение внедрённого типа</summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(Injector),
            new PropertyMetadata(default(object)));

    /// <summary>Отображение внедрённого типа</summary>
    [Category("Injector")]
    [Description("Отображение внедрённого типа")]
    public object? Content
    {
        get => GetValue(ContentProperty);
        private set => SetValue(ContentProperty, value);
    }

    #endregion
}