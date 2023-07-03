using System;
using System.ComponentModel;
using System.Windows;
using WPR.Dialogs.Base;
using WPR.Domain.Interfaces;

namespace WPR.Dialogs;

/// <summary>
/// Диалог с произвольными кнопками
/// </summary>
public class CustomButtonsDialog : DialogBase, IWPRDialog
{
    static CustomButtonsDialog()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomButtonsDialog), new FrameworkPropertyMetadata(typeof(CustomButtonsDialog)));
    }


    public CustomButtonsDialog()
    {
        DialogResult += B =>
        {
            DialogBaseResult = B;
#pragma warning disable CS0612
            SetDialogResult?.Invoke(true);
#pragma warning restore CS0612
        };
    }


    #region Caption : string - Описание

    /// <summary>Описание</summary>
    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(
            nameof(Caption),
            typeof(string),
            typeof(CustomButtonsDialog),
            new PropertyMetadata(default(string)));

    /// <summary>Описание</summary>
    [Category("CustomButtonsDialog")]
    [Description("Описание")]
    public string Caption
    {
        get => (string) GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    #endregion


    #region AcceptButtonText : string - Текст кнопки подтверждения

    /// <summary>Текст кнопки подтверждения</summary>
    public static readonly DependencyProperty AcceptButtonTextProperty =
        DependencyProperty.Register(
            nameof(AcceptButtonText),
            typeof(string),
            typeof(CustomButtonsDialog),
            new PropertyMetadata(default(string)));

    /// <summary>Текст кнопки подтверждения</summary>
    [Category("CustomButtonsDialog")]
    [Description("Текст кнопки подтверждения")]
    public string AcceptButtonText
    {
        get => (string) GetValue(AcceptButtonTextProperty);
        set => SetValue(AcceptButtonTextProperty, value);
    }

    #endregion


    #region DeclineButtonText : string - Текст кнопки отказа

    /// <summary>Текст кнопки отказа</summary>
    public static readonly DependencyProperty DeclineButtonTextProperty =
        DependencyProperty.Register(
            nameof(DeclineButtonText),
            typeof(string),
            typeof(CustomButtonsDialog),
            new PropertyMetadata(default(string)));

    /// <summary>Текст кнопки отказа</summary>
    [Category("CustomButtonsDialog")]
    [Description("Текст кнопки отказа")]
    public string DeclineButtonText
    {
        get => (string) GetValue(DeclineButtonTextProperty);
        set => SetValue(DeclineButtonTextProperty, value);
    }

    #endregion


    #region CancelButtonText : string - Текст кнопки отмены

    /// <summary>Текст кнопки отмены</summary>
    public static readonly DependencyProperty CancelButtonTextProperty =
        DependencyProperty.Register(
            nameof(CancelButtonText),
            typeof(string),
            typeof(CustomButtonsDialog),
            new PropertyMetadata(default(string)));

    /// <summary>Текст кнопки отмены</summary>
    [Category("CustomButtonsDialog")]
    [Description("Текст кнопки отмены")]
    public string CancelButtonText
    {
        get => (string) GetValue(CancelButtonTextProperty);
        set => SetValue(CancelButtonTextProperty, value);
    }

    #endregion

    /// <summary> В этом классе всегда будет возвращать истину! Используй DialogBaseResult  </summary>
    [Obsolete]
    public Action<bool> SetDialogResult { get; set; }

    /// <summary> Результат выполнения базового диалога </summary>
    public bool? DialogBaseResult { get; set; }


    public event Action<bool> Completed;
}