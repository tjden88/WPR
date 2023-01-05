using System.Windows;
using WPR.Domain.Interfaces;

namespace WPR.UiServices.Interfaces;

/// <summary> Сервис навигации и поиска представлений </summary>
public interface IAppNavigation
{
    /// <summary> Активное окно приложения </summary>
    Window? ActiveWindow { get; }

    /// <summary> Получить представление из коллекции сервисов </summary>
    public T GetView<T>() where T : IView;

    /// <summary> Получить окно необходимого типа </summary>
    T GetWindow<T>() where T : Window;

    /// <summary> Получить окно необходимого типа, заданное как модальное окно для активного окна </summary>
    T GetModalWindow<T>() where T : Window;
}