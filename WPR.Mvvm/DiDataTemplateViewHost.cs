using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace WPR.Mvvm;

/// <summary>
/// Контрол-хост, который создаёт View через DI (ActivatorUtilities) и подставляет текущий DataContext.
/// </summary>
/// <remarks>
/// Нужен, чтобы DataTemplate мог создавать View с DI-конструктором (например, SceneView(HelixViewportFacade)),
/// при этом сохраняя исходный экземпляр ViewModel из привязки.
/// Для scoped-сцен берёт IServiceProvider из DataContext (IScopedServiceProviderAccessor).
/// </remarks>
public sealed class DiDataTemplateViewHost : ContentControl
{
    /// <summary>
    /// Тип создаваемого представления (UserControl).
    /// </summary>
    public Type? ViewType
    {
        get => (Type?)GetValue(ViewTypeProperty);
        set => SetValue(ViewTypeProperty, value);
    }

    /// <summary>
    /// DependencyProperty для <see cref="ViewType"/>.
    /// </summary>
    public static readonly DependencyProperty ViewTypeProperty =
        DependencyProperty.Register(
            nameof(ViewType),
            typeof(Type),
            typeof(DiDataTemplateViewHost),
            new PropertyMetadata(null, PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DiDataTemplateViewHost)d).RebuildContent();
    }

    /// <summary>
    /// Пересоздаёт содержимое (View) с учётом текущих <see cref="ViewType"/> и <see cref="FrameworkElement.DataContext"/>.
    /// </summary>
    private void RebuildContent()
    {
        if (ViewType is null || DataContext is null)
        {
            Content = null;
            return;
        }

        if (!typeof(FrameworkElement).IsAssignableFrom(ViewType))
            throw new InvalidOperationException($"ViewType должен быть FrameworkElement. Получено: {ViewType.FullName}");

        var sp = ServiceResolver.Services;

        // Создаём View через DI (работают конструкторы с зависимостями)
        var view = (FrameworkElement)ActivatorUtilities.CreateInstance(sp, ViewType);

        // Самое важное: используем ТОТ ЖЕ экземпляр VM, который пришёл из Binding
        view.DataContext = DataContext;

        Content = view;
    }

}