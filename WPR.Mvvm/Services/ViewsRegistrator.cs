using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using WPR.Mvvm.Interfaces;

namespace WPR.Mvvm.Services;

/// <summary>
/// Регистратор представлений и моделей-представлений:
/// 1) Регистрирует ViewModel в DI с указанным временем жизни.
/// 2) Добавляет DataTemplate (ViewModel -> View) в <see cref="ResourceDictionary"/>.
/// 3) Регистрирует фабрику View/Window для использования инжектором/навигацией.
/// </summary>
/// <remarks>
/// ВАЖНО: View/Window создаются через DI (ActivatorUtilities), поэтому конструктор по умолчанию не требуется.
/// </remarks>
internal sealed class ViewsRegistrator(IServiceCollection serviceCollection) : IViewsRegistrator
{
    /// <summary>
    /// Подготовленный для добавления в ресурсы приложения словарь ресурсов.
    /// </summary>
    public ResourceDictionary ResourceDictionary { get; } = new();

    #region Public API: Views

    /// <inheritdoc />
    public IViewsRegistrator AddSingleton<TView, TViewModel>(string? Name = null)
        where TView : FrameworkElement
        => AddView<TView>(typeof(TViewModel), Name, ServiceLifetime.Singleton);

    /// <inheritdoc />
    public IViewsRegistrator AddSingleton<TView>(string? Name = null)
        where TView : FrameworkElement
        => AddView<TView>(FindViewModelTypeOrThrow(typeof(TView)), Name, ServiceLifetime.Singleton);

    /// <inheritdoc />
    public IViewsRegistrator AddTransient<TView, TViewModel>(string? Name = null)
        where TView : FrameworkElement
        => AddView<TView>(typeof(TViewModel), Name, ServiceLifetime.Transient);

    /// <inheritdoc />
    public IViewsRegistrator AddTransient<TView>(string? Name = null)
        where TView : FrameworkElement
        => AddView<TView>(FindViewModelTypeOrThrow(typeof(TView)), Name, ServiceLifetime.Transient);

    /// <inheritdoc />
    public IViewsRegistrator AddScoped<TView, TViewModel>(string? Name = null)
        where TView : FrameworkElement
        => AddView<TView>(typeof(TViewModel), Name, ServiceLifetime.Scoped);

    /// <inheritdoc />
    public IViewsRegistrator AddScoped<TView>(string? Name = null)
        where TView : FrameworkElement
        => AddView<TView>(FindViewModelTypeOrThrow(typeof(TView)), Name, ServiceLifetime.Scoped);

    /// <inheritdoc />
    public IViewsRegistrator AddTemplateOnly<TView>()
        where TView : FrameworkElement
    {
        var viewType = typeof(TView);
        var vmType = FindViewModelTypeOrThrow(viewType);
        AddDataTemplate(viewType, vmType);
        return this;
    }

    /// <inheritdoc />
    public IViewsRegistrator AddTemplateOnly<TView, TViewModel>()
        where TView : FrameworkElement
    {
        AddDataTemplate(typeof(TView), typeof(TViewModel));
        return this;
    }

    #endregion

    #region Public API: Windows

    /// <inheritdoc />
    public IViewsRegistrator AddTransientWindow<TWindow, TViewModel>(string? Name = null)
        where TWindow : Window
        => AddWindow<TWindow>(typeof(TViewModel), Name, ServiceLifetime.Transient, isMainWindow: false);

    /// <inheritdoc />
    public IViewsRegistrator AddMainWindow<TWindow, TViewModel>(string? Name = null)
        where TWindow : Window
        => AddWindow<TWindow>(typeof(TViewModel), Name, ServiceLifetime.Singleton, isMainWindow: true);

    #endregion

    #region Core registration

    /// <summary>
    /// Регистрирует View + ViewModel и добавляет DataTemplate.
    /// </summary>
    private IViewsRegistrator AddView<TView>(Type viewModelType, string? registerName, ServiceLifetime lifetime)
        where TView : FrameworkElement
    {
        var viewType = typeof(TView);
        var name = registerName ?? viewModelType.Name;

        RegisterViewModel(name, viewModelType);
        AddViewModel(viewModelType, lifetime);

        // View создаётся через DI, чтобы не требовать конструктор по умолчанию.
        serviceCollection.AddTransient(sp =>
        {
            var vm = sp.GetRequiredService(viewModelType);
            var view = ActivatorUtilities.CreateInstance<TView>(sp);
            view.DataContext = vm;
            return view;
        });

        AddDataTemplate(viewType, viewModelType);
        return this;
    }

    /// <summary>
    /// Регистрирует Window + ViewModel и добавляет DataTemplate.
    /// </summary>
    private IViewsRegistrator AddWindow<TWindow>(Type viewModelType, string? registerName, ServiceLifetime lifetime, bool isMainWindow)
        where TWindow : Window
    {
        var windowType = typeof(TWindow);
        var name = registerName ?? viewModelType.Name;

        RegisterViewModel(name, viewModelType);
        AddViewModel(viewModelType, lifetime);

        // Окно тоже создаём через DI.
        if (lifetime == ServiceLifetime.Singleton)
        {
            serviceCollection.AddSingleton(sp => CreateWindow<TWindow>(sp, viewModelType, isMainWindow));
        }
        else
        {
            serviceCollection.AddTransient(sp => CreateWindow<TWindow>(sp, viewModelType, isMainWindow));
        }

        AddDataTemplate(windowType, viewModelType);
        return this;
    }

    /// <summary>
    /// Создаёт окно через DI и назначает DataContext.
    /// </summary>
    private static TWindow CreateWindow<TWindow>(IServiceProvider sp, Type viewModelType, bool isMainWindow)
        where TWindow : Window
    {
        var vm = sp.GetRequiredService(viewModelType);
        var window = ActivatorUtilities.CreateInstance<TWindow>(sp);
        window.DataContext = vm;

        if (isMainWindow)
        {
            window.Closed += (_, _) => Application.Current.Shutdown();
        }

        // Если VM disposable, освобождаем при закрытии окна.
        if (vm is IDisposable disposable)
            window.Closed += (_, _) => disposable.Dispose();

        return window;
    }

    /// <summary>
    /// Регистрирует ViewModel в DI согласно <paramref name="lifetime"/>.
    /// </summary>
    private void AddViewModel(Type viewModelType, ServiceLifetime lifetime)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                serviceCollection.AddSingleton(viewModelType);
                break;
            case ServiceLifetime.Scoped:
                serviceCollection.AddScoped(viewModelType);
                break;
            case ServiceLifetime.Transient:
                serviceCollection.AddTransient(viewModelType);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }
    }

    #endregion

    #region ViewModel discovery/registry

    /// <summary>
    /// Ищет тип ViewModel по соглашению об именовании: {ИмяView}ViewModel.
    /// </summary>
    private static Type FindViewModelTypeOrThrow(Type viewType)
    {
        var expectedName = $"{viewType.Name}ViewModel";

        var vmType = viewType.Assembly
            .GetTypes()
            .FirstOrDefault(t => t.Name == expectedName);

        return vmType ?? throw new InvalidOperationException(
            $"Модель представления для типа {viewType.Name} не найдена. Искомое имя - {expectedName}");
    }

    /// <summary>
    /// Регистрирует ViewModel в реестре MVVM, чтобы можно было искать типы по строковому имени.
    /// </summary>
    private static void RegisterViewModel(string name, Type type)
    {
        if (!Mvvm.RegisteredViews.TryAdd(name, type))
            throw new ArgumentException("Такое имя уже зарегистрировано", nameof(name));
    }

    #endregion

    #region DataTemplate

    /// <summary>
    /// Добавляет DataTemplate (ViewModel -> View) в ресурсы.
    /// </summary>
    private void AddDataTemplate(Type view, Type viewModel)
    {
        var stringReader = new StringReader(
            @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                xmlns:vm=""clr-namespace:" + viewModel.Namespace + @";assembly=" + viewModel.Assembly.GetName().Name + @"""
                                xmlns:v=""clr-namespace:" + view.Namespace + @";assembly=" + view.Assembly.GetName().Name + @"""
                                DataType=""{x:Type vm:" + viewModel.Name + @"}"">
                    <v:" + view.Name + @" DataContext=""{Binding}""/>
                </DataTemplate>");

        var xmlReader = XmlReader.Create(stringReader);
        var template = (DataTemplate)XamlReader.Load(xmlReader);

        ResourceDictionary.Add(template.DataTemplateKey!, template);
    }

    #endregion
}
