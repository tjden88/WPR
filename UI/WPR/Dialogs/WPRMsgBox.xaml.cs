using System.ComponentModel;
using System.Windows;
using WPR.Dialogs.Base;

namespace WPR.Dialogs;

public class WPRMsgBox : DialogBase
{
    static WPRMsgBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRMsgBox), new FrameworkPropertyMetadata(typeof(WPRMsgBox)));
    }

    #region Caption : string - Текст сообщения

    /// <summary>Текст сообщения</summary>
    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(
            nameof(Caption),
            typeof(string),
            typeof(WPRMsgBox),
            new PropertyMetadata(default(string)));

    /// <summary>Текст сообщения</summary>
    //[Category("")]
    [Description("Текст сообщения")]
    public string Caption
    {
        get => (string) GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    #endregion

    #region CancelButtonVisible : bool - Видимость кнопки отмены

    /// <summary>Видимость кнопки отмены</summary>
    public static readonly DependencyProperty CancelButtonVisibleProperty =
        DependencyProperty.Register(
            nameof(CancelButtonVisible),
            typeof(bool),
            typeof(WPRMsgBox),
            new PropertyMetadata(default(bool)));

    /// <summary>Видимость кнопки отмены</summary>
    //[Category("")]
    [Description("Видимость кнопки отмены")]
    public bool CancelButtonVisible
    {
        get => (bool) GetValue(CancelButtonVisibleProperty);
        set => SetValue(CancelButtonVisibleProperty, value);
    }

    #endregion

    #region YesNoButtonsVisible : bool - Видимость кнопок Да,Нет. Если false - видна кнопка OK

    /// <summary>Видимость кнопок Да,Нет. Если false - видна кнопка OK</summary>
    public static readonly DependencyProperty YesNoButtonsVisibleProperty =
        DependencyProperty.Register(
            nameof(YesNoButtonsVisible),
            typeof(bool),
            typeof(WPRMsgBox),
            new PropertyMetadata(default(bool)));

    /// <summary>Видимость кнопок Да,Нет. Если false - видна кнопка OK</summary>
    //[Category("")]
    [Description("Видимость кнопок Да,Нет. Если false - видна кнопка OK")]
    public bool YesNoButtonsVisible
    {
        get => (bool)GetValue(YesNoButtonsVisibleProperty);
        set => SetValue(YesNoButtonsVisibleProperty, value);
    }

    #endregion

}