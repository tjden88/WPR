using System.Windows;

namespace WPR.UiServices.Interfaces;

/// <summary> Сервис навигации и поиска представлений </summary>
public interface IAppNavigation
{
    /// <summary> Активное окно приложения </summary>
    Window? ActiveWindow { get; }


    /// <summary> Получить окно необходимого типа </summary>
    T GetWindow<T>() where T : Window;

    /// <summary> Получить окно необходимого типа, заданное как модальное окно для активного окна </summary>
    T GetModalWindow<T>() where T : Window;
}