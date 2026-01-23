using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using WPR.Mvvm.Interfaces;
using WPR.Mvvm.Services;

namespace WPR.Mvvm;

/// <summary>
/// Базовый класс WPF-приложения с DI и автоматическим связыванием ViewModel -> View через DataTemplate.
/// </summary>
/// <remarks>
/// Контейнер собирается строго один раз в OnStartup.
/// Никаких ленивых "построим когда-нибудь потом" и статических регистраций до старта приложения.
/// </remarks>
public abstract class MvvmApp : Application
{
    /// <summary>
    /// Корневой провайдер сервисов приложения.
    /// </summary>
    /// <remarks>
    /// Используй это, если инфраструктуре реально нужен доступ к контейнеру.
    /// Для обычных зависимостей предпочитай конструкторную инъекцию.
    /// </remarks>
    protected static IServiceProvider Services => ServiceResolver.Services;

    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        ApplyCulture();

        base.OnStartup(e);

        // 1) Собираем коллекцию сервисов локально, без глобальных статиков.
        var services = new ServiceCollection();

        // 2) Пользовательская конфигурация сервисов.
        ConfigureServices(services);

        // 3) Инфраструктурные сервисы MVVM (как и было, но без статической ServiceCollection).
        services.AddSingleton(typeof(IResolver<>), typeof(Resolver<>));

        // 4) Регистрируем View/ViewModel и шаблоны (DataTemplate).
        var viewsRegistrator = new ViewsRegistrator(services);
        RegisterViews(viewsRegistrator);
        Resources.MergedDictionaries.Add(viewsRegistrator.ResourceDictionary);

        // 5) Строим provider один раз, с проверкой scope-ошибок.
        var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });

        // 6) Публикуем provider для инфраструктуры (Injector и т.д.).
        ServiceResolver.Initialize(provider);

        // 7) Если у тебя главное окно регистрируется через ViewsRegistrator:
        // var mainWindow = provider.GetRequiredService<MainWindow>();
        // mainWindow.Show();
        //
        // ВАЖНО: Если ты используешь StartupUri в App.xaml, то оно само создаст окно через WPF,
        // и DI-конструкторы окна не будут работать
        OnAfterStartup(provider, e);
    }

    /// <inheritdoc />
    protected override void OnExit(ExitEventArgs e)
    {
        // Освобождаем корневой provider, чтобы IDisposable-сервисы (рендер, файлы, подписки) закрылись корректно.
        ServiceResolver.Dispose();
        base.OnExit(e);
    }
    /// <summary>
    /// Точка расширения: вызывается после сборки DI-контейнера и регистрации ресурсов.
    /// Здесь можно открыть главное окно, показать сплэш, выполнить миграции и т.д.
    /// </summary>
    /// <param name="services">Корневой провайдер сервисов.</param>
    /// <param name="e">Аргументы запуска приложения.</param>
    protected abstract void OnAfterStartup(IServiceProvider services, StartupEventArgs e);


    /// <summary>
    /// Конфигурация сервисов приложения в контейнер сервисов.
    /// </summary>
    protected abstract void ConfigureServices(IServiceCollection services);

    /// <summary>
    /// Регистрация представлений и связанных с ними моделей-представлений.
    /// </summary>
    protected abstract void RegisterViews(IViewsRegistrator viewsRegistrator);

    /// <summary>
    /// Применяет культуру к текущему потоку и WPF (LanguageProperty).
    /// </summary>
    private static void ApplyCulture()
    {
        var ci = CultureInfo.GetCultureInfo("ru-RU");

        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;

        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(ci.IetfLanguageTag)));
    }
}
