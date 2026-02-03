using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;

namespace WPR.Mvvm;

/// <summary>
///     MarkupExtension для получения сервиса из DI-контейнера в XAML.
///     Пример: DataContext="{di:Resolve Type={x:Type vm:FileToolbarViewModel}}"
/// </summary>
[MarkupExtensionReturnType(typeof(object))]
public sealed class ResolveExtension : MarkupExtension
{

    public ResolveExtension() { }

    public ResolveExtension(Type type) => Type = type;

    /// <summary>
    ///     Тип сервиса, который нужно получить из DI.
    /// </summary>
    public Type? Type { get; set; }

    /// <summary>
    ///     Разрешить сервис из DI-контейнера.
    /// </summary>
    /// <param name="serviceProvider">Сервис-провайдер разметки WPF.</param>
    /// <returns>Экземпляр сервиса из DI.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Если глобальный ServiceProvider не установлен, либо Type не задан.
    /// </exception>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Type is null)
            throw new InvalidOperationException("ResolveExtension: не задано свойство Type.");

        if (!ServiceResolver.IsInitialized) // Работаем в дизайнере
        {
            var content = Activator.CreateInstance(Type);
            return content ?? $"Не удалось создать тип {Type}";
        }

        // GetRequiredService бросит понятное исключение, если сервис не зарегистрирован.
        return ServiceResolver.Services.GetRequiredService(Type);
    }
}