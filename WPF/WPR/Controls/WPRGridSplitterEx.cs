#nullable enable
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPR.Controls;

/// <summary>
/// </summary>
public class WPRGridSplitterEx : GridSplitter
{
    //static WPRGridSplitterEx()
    //{
    //    DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRGridSplitterEx), new FrameworkPropertyMetadata(typeof(WPRGridSplitterEx)));
    //}

    #region Tagret : FrameworkElement - Цель изменения размера

    /// <summary>Цель изменения размера</summary>
    public static readonly DependencyProperty TagretProperty =
        DependencyProperty.Register(
            nameof(Tagret),
            typeof(FrameworkElement),
            typeof(WPRGridSplitterEx),
            new PropertyMetadata(default(FrameworkElement)));

    /// <summary>Цель изменения размера</summary>
    [Category("WPRGridSplitterEx")]
    [Description("Цель изменения размера")]
    public FrameworkElement Tagret
    {
        get => (FrameworkElement)GetValue(TagretProperty);
        set => SetValue(TagretProperty, value);
    }

    #endregion

    #region TargetMinWidth : double - Минимальный размер цели

    /// <summary>Минимальный размер цели</summary>
    public static readonly DependencyProperty TargetMinWidthProperty =
        DependencyProperty.Register(
            nameof(TargetMinWidth),
            typeof(double),
            typeof(WPRGridSplitterEx),
            new PropertyMetadata(default(double)));

    /// <summary>Минимальный размер цели</summary>
    [Category("WPRGridSplitterEx")]
    [Description("Минимальный размер цели")]
    public double TargetMinWidth
    {
        get => (double)GetValue(TargetMinWidthProperty);
        set => SetValue(TargetMinWidthProperty, value);
    }

    #endregion

    private double _Delta;
    private FrameworkElement? _Parent = null;
    private double _TargetWidth;

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        _Parent = this.FindVisualParent<Window>();
        if (_Parent == null) return;

        _Delta = e.GetPosition(_Parent).X;
        _TargetWidth = Tagret.ActualWidth;
        CaptureMouse();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!IsMouseCaptured) return;

        var position = e.GetPosition(_Parent).X;
        var delta = position - _Delta;

        Tagret.Width = _TargetWidth - delta;

    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        ReleaseMouseCapture();
    }
}