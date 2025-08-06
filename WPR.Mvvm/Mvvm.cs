using System.Windows;
using System.Windows.Controls;

namespace WPR.Mvvm;

public static class Mvvm
{
    /// <summary>
    /// Словарь областей и назначенных им контролов
    /// </summary>
    internal static readonly Dictionary<string, ContentControl> AttachedRegions = new();

    /// <summary>
    /// Словарь зарегистрированных моделей-представлений
    /// </summary>
    internal static readonly Dictionary<string, Type> RegisteredViews = new();



    #region Attached property Region : string - Область навигации

    /// <summary>Область навигации</summary>
    public static readonly DependencyProperty RegionProperty =
        DependencyProperty.RegisterAttached(
            "Region",
            typeof(string),
            typeof(Mvvm),
            new PropertyMetadata(null, PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        AttachedRegions[(string) E.NewValue] = (ContentControl) D;
    }

    /// <summary>Область навигации</summary>
    [AttachedPropertyBrowsableForType(typeof(ContentControl))]
    public static void SetRegion(DependencyObject d, string value) => d.SetValue(RegionProperty, value);

    /// <summary>Область навигации</summary>
    public static string GetRegion(DependencyObject d) => (string) d.GetValue(RegionProperty);

    #endregion


    #region Attached property Content : string - Установить контент области автоматически

    /// <summary>Установить контент области автоматически</summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.RegisterAttached(
            "Content",
            typeof(string),
            typeof(Mvvm),
            new PropertyMetadata(null, ContentChangedCallback));

    private static void ContentChangedCallback(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        Navigator.SetContent((ContentControl) D, (string) E.NewValue);
    }

    /// <summary>Установить контент области автоматически</summary>
    [AttachedPropertyBrowsableForType(typeof(ContentControl))]
    public static void SetContent(DependencyObject d, string value) => d.SetValue(ContentProperty, value);

    /// <summary>Установить контент области автоматически</summary>
    public static string GetContent(DependencyObject d) => (string) d.GetValue(ContentProperty);

    #endregion

  
}