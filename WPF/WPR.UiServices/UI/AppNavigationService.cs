using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WPR.UiServices.Interfaces;

namespace WPR.UiServices.UI;

/// <summary> Сервис навигации и поиска представлений </summary>
public class AppNavigationService : IAppNavigation
{
    private readonly IServiceProvider _Services;

    public AppNavigationService(IServiceProvider Services)
    {
        _Services = Services;
    }


    /// <summary> Получить активное окно </summary>
    public Window? ActiveWindow => Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);


    public T GetView<T>() where T : notnull => _Services.GetRequiredService<T>();


    public T GetWindow<T>() where T : Window => _Services.GetRequiredService<T>();

    public T GetModalWindow<T>() where T : Window
    {
        var wnd = _Services.GetRequiredService<T>();
        wnd.Owner = ActiveWindow;
        return wnd;
    }
}