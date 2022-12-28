using System.Windows;
using WPR.UiServices.Interfaces;

namespace WPR.UiServices.UI;

/// <summary> Сервис навигации и поиска представлений </summary>
public class AppNavigationService : IAppNavigation
{
    /// <summary> Получить активное окно </summary>
    public Window? ActiveWindow => Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);

    public T GetWindow<T>() where T : Window
    {
        throw new NotImplementedException();
    }

    public T GetModalWindow<T>() where T : Window
    {
        throw new NotImplementedException();
    }
}