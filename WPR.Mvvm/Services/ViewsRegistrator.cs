using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using WPR.Mvvm.Interfaces;

namespace WPR.Mvvm.Services;

internal class ViewsRegistrator(IServiceCollection ServiceCollection) : IViewsRegistrator
{
    /// <summary>
    /// Подготовленный для добавления в ресурсы приложения словарь ресурсов
    /// </summary>
    public ResourceDictionary ResourceDictionary { get; } = new();


    public IViewsRegistrator AddSingleton<TView, TViewModel>(string? Name = null) where TView : FrameworkElement, new()
    {
        var type = typeof(TViewModel);
        var name = Name ?? type.Name;
        Register<TView>(type, name, ServiceLifetime.Singleton);

        return this;
    }

    public IViewsRegistrator AddSingleton<TView>(string? Name = null) where TView : FrameworkElement, new()
    {
        var type = typeof(TView);
        var viewModelType = type.Assembly.GetTypes().FirstOrDefault(t => t.Name == $"{type.Name}ViewModel") ?? throw new InvalidOperationException($"Модель представления для типа {type.Name} не найдена. Искомое имя - {type.Name}ViewModel");
        Register<TView>(viewModelType, Name ?? viewModelType.Name, ServiceLifetime.Singleton);

        return this;
    }


    public IViewsRegistrator AddTransient<TView, TViewModel>(string? Name = null) where TView : FrameworkElement, new()
    {
        var type = typeof(TViewModel);
        var name = Name ?? type.Name;
        Register<TView>(type, name, ServiceLifetime.Transient);
        return this;
    }

    public IViewsRegistrator AddTransient<TView>(string? Name = null) where TView : FrameworkElement, new()
    {
        var type = typeof(TView);
        var viewModelType = type.Assembly.GetTypes().FirstOrDefault(t => t.Name == $"{type.Name}ViewModel") ?? throw new InvalidOperationException($"Модель представления для типа {type.Name} не найдена. Искомое имя - {type.Name}ViewModel");

        Register<TView>(viewModelType, Name ?? viewModelType.Name, ServiceLifetime.Transient);

        return this;
    }

    public IViewsRegistrator AddScoped<TView, TViewModel>(string? Name = null) where TView : FrameworkElement, new()
    {
        var type = typeof(TViewModel);
        var name = Name ?? type.Name;
        Register<TView>(type, name, ServiceLifetime.Scoped);
        return this;
    }

    public IViewsRegistrator AddScoped<TView>(string? Name = null) where TView : FrameworkElement, new()
    {
        var type = typeof(TView);
        var viewModelType = type.Assembly.GetTypes().FirstOrDefault(t => t.Name == $"{type.Name}ViewModel") ?? throw new InvalidOperationException($"Модель представления для типа {type.Name} не найдена. Искомое имя - {type.Name}ViewModel");
        Register<TView>(viewModelType, Name ?? viewModelType.Name, ServiceLifetime.Scoped);
        return this;
    }


    public IViewsRegistrator AddTransientWindow<TWindow, TViewModel>(string? Name = null) where TWindow : Window, new()
    {
        var viewModelType = typeof(TViewModel);
        var windowType = typeof(TWindow);
        var name = Name ?? viewModelType.Name;

        RegisterViewModelDataTemplate(name, viewModelType);


        ServiceCollection.AddTransient(windowType);

        ServiceCollection.AddTransient(s =>
        {
            var viewModel = s.GetRequiredService(viewModelType);
            var view = new TWindow
            {
                DataContext = viewModel
            };
            if (viewModel is IDisposable disposable)
                view.Closed += (_, _) => disposable.Dispose();

            return view;
        });
        ServiceCollection.AddTransient(viewModelType);

        AddDataTemplate(windowType, viewModelType);
        return this;
    }

    public IViewsRegistrator AddMainWindow<TWindow, TViewModel>(string? Name = null) where TWindow : Window, new()
    {
        var viewModelType = typeof(TViewModel);
        var windowType = typeof(TWindow);
        var name = Name ?? viewModelType.Name;

        RegisterViewModelDataTemplate(name, viewModelType);


        ServiceCollection.AddSingleton(windowType);

        ServiceCollection.AddSingleton(s =>
        {
            var viewModel = s.GetRequiredService(viewModelType);
            var view = new TWindow
            {
                DataContext = viewModel
            };

            view.Closed += (_, _) => Application.Current.Shutdown();
            return view;
        });

        ServiceCollection.AddSingleton(viewModelType);

        AddDataTemplate(windowType, viewModelType);
        return this;
    }

    public IViewsRegistrator AddTemplateOnly<TView>() where TView : FrameworkElement, new()
    {
        var type = typeof(TView);
        var viewModelType = type.Assembly.GetTypes().FirstOrDefault(t => t.Name == $"{type.Name}ViewModel") ?? throw new InvalidOperationException($"Модель представления для типа {type.Name} не найдена. Искомое имя - {type.Name}ViewModel");
        AddDataTemplate(type, viewModelType);
        return this;
    }

    public IViewsRegistrator AddTemplateOnly<TView, TViewModel>() where TView : FrameworkElement, new()
    {
        var type = typeof(TViewModel);
        AddDataTemplate(typeof(TView), type);
        return this;
    }


    /// <summary>
    /// Регистрация представления и модели-представления в коллекции сервисов.
    /// </summary>
    private void Register<TView>(Type ViewModelType, string RegisterName, ServiceLifetime Lifetime) where TView : FrameworkElement, new()
    {
        RegisterViewModelDataTemplate(RegisterName, ViewModelType);

        switch (Lifetime) // Регистрация модели-представления в зависимости от жизненного цикла
        {
            case ServiceLifetime.Singleton:
                ServiceCollection.AddSingleton(ViewModelType);
                break;
            case ServiceLifetime.Scoped:
                ServiceCollection.AddScoped(ViewModelType);
                break;
            case ServiceLifetime.Transient:
                ServiceCollection.AddTransient(ViewModelType);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Lifetime), Lifetime, null);
        }

        ServiceCollection.AddTransient(sp => // View для инжектора
        {
            var vm = sp.GetRequiredService(ViewModelType);
            var view = new TView // Создание нового экземпляра представления
            {
                DataContext = vm
            };

            return view;
        });

        AddDataTemplate(typeof(TView), ViewModelType);
    }


    // Зарегистрировать модель-представление в словаре
    private void RegisterViewModelDataTemplate(string Name, Type type)
    {
        if (!Mvvm.RegisteredViews.TryAdd(Name, type))
            throw new ArgumentException("Такое имя уже зарегистрировано", nameof(Name));
    }

    // Добавить шаблон данных в ресурсы
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
}