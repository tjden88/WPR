using System.ComponentModel;
using System.Windows;
using WPR.Dialogs.Base;

namespace WPR.Dialogs;

public class MessageDialog : DialogBase
{
    static MessageDialog()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialog), new FrameworkPropertyMetadata(typeof(MessageDialog)));
    }

    #region Caption : string - Текст сообщения

    /// <summary>Текст сообщения</summary>
    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(
            nameof(Caption),
            typeof(string),
            typeof(MessageDialog),
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
            typeof(MessageDialog),
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
            typeof(MessageDialog),
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

    #region IsErrorMessage : bool - Истино, если это сообщение об ошибке

    /// <summary>Истино, если это сообщение об ошибке</summary>
    public static readonly DependencyProperty IsErrorMessageProperty =
        DependencyProperty.Register(
            nameof(IsErrorMessage),
            typeof(bool),
            typeof(MessageDialog),
            new PropertyMetadata(default(bool)));

    /// <summary>Истино, если это сообщение об ошибке</summary>
    [Category("MessageDialog")]
    [Description("Истино, если это сообщение об ошибке")]
    public bool IsErrorMessage
    {
        get => (bool) GetValue(IsErrorMessageProperty);
        set => SetValue(IsErrorMessageProperty, value);
    }

    #endregion
}