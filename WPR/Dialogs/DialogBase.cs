using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.MVVM.Commands;
using WPR.MVVM.Commands.Base;

namespace WPR.Dialogs;

public abstract class DialogBase : Control
{
    /// <summary>Происходит при завершении ввода пользователя</summary>
    public Action<bool?> DialogResult;

    protected DialogBase()
    {
        SetDialogResultCommand = new Command(obj => DialogResult?.Invoke((bool) obj),_ => CanSetCommandExecuted());
        CancelCommand = new Command(() => DialogResult?.Invoke(null));
    }

    #region Title : string - Заголовок

    /// <summary>Заголовок</summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(DialogBase),
            new PropertyMetadata(default(string)));

    /// <summary>Заголовок</summary>
    [Description("Заголовок")]
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    #endregion


    #region SetDialogResultCommand : ICommand - Команда нажатия контрольных кнопок

    /// <summary>Команда нажатия контрольных кнопок</summary>
    public static readonly DependencyProperty SetDialogResultCommandProperty =
        DependencyProperty.Register(
            nameof(SetDialogResultCommand),
            typeof(ICommand),
            typeof(DialogBase),
            new PropertyMetadata(null));

    /// <summary>Команда нажатия контрольных кнопок</summary>
    //[Category("")]
    [Description("Команда нажатия контрольных кнопок")]
    public ICommand SetDialogResultCommand
    {
        get => (ICommand)GetValue(SetDialogResultCommandProperty);
        set => SetValue(SetDialogResultCommandProperty, value);
    }
    #endregion


    #region CancelCommand : ICommand - Команда отмены

    /// <summary>Команда отмены</summary>
    public static readonly DependencyProperty CancelCommandProperty =
        DependencyProperty.Register(
            nameof(CancelCommand),
            typeof(ICommand),
            typeof(DialogBase),
            new PropertyMetadata(null));

    /// <summary>Команда отмены</summary>
    //[Category("")]
    [Description("Команда отмены")]
    public ICommand CancelCommand
    {
        get => (ICommand)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }
    #endregion


    /// <summary>Может ли выполниться команда SetDialogResultCommand</summary>
    protected virtual bool CanSetCommandExecuted() => true;
}