﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPR.Icons;

namespace WPR.Controls;

/// <summary>
/// Текст с иконкой слева
/// </summary>
public class WPRIconText : Control
{
    static WPRIconText()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRIconText), new FrameworkPropertyMetadata(typeof(WPRIconText)));
    }

    #region Text : string - Текст 

    /// <summary>Текст </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(WPRIconText),
            new PropertyMetadata(default(string)));

    /// <summary>Текст </summary>
    [Category("WPRIconText")]
    [Description("Текст ")]
    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    #endregion

    #region IconSource : PackIconKind - Иконка с текстом

    /// <summary>Иконка с текстом</summary>
    public static readonly DependencyProperty IconSourceProperty =
        DependencyProperty.Register(
            nameof(IconSource),
            typeof(PackIconKind),
            typeof(WPRIconText),
            new PropertyMetadata(default(PackIconKind)));

    /// <summary>Иконка с текстом</summary>
    [Category("WPRIconText")]
    [Description("Иконка с текстом")]
    public PackIconKind IconSource
    {
        get => (PackIconKind) GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    #endregion

    #region IconForeground : Brush - Цвет иконки

    /// <summary>Цвет иконки</summary>
    public static readonly DependencyProperty IconForegroundProperty =
        DependencyProperty.Register(
            nameof(IconForeground),
            typeof(Brush),
            typeof(WPRIconText),
            new PropertyMetadata(default(Brush)));

    /// <summary>Цвет иконки</summary>
    [Category("WPRIconText")]
    [Description("Цвет иконки")]
    public Brush IconForeground
    {
        get => (Brush) GetValue(IconForegroundProperty);
        set => SetValue(IconForegroundProperty, value);
    }

    #endregion  
}