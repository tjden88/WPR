#nullable enable
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPR.Controls;

/// <summary>
/// Изменение размера связанного элемента при помощи мыши
/// </summary>
public class WPRGridSplitterEx : GridSplitter
{
    /// <summary> Расположения элемента относительно целевого объекта </summary>
    public enum Placements
    {
        Left,
        Top,
        Right, 
        Bottom,
    }

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


    #region TargetMinSize : double - Минимальный размер цели

    /// <summary>Минимальный размер цели</summary>
    public static readonly DependencyProperty TargetMinSizeProperty =
        DependencyProperty.Register(
            nameof(TargetMinSize),
            typeof(double),
            typeof(WPRGridSplitterEx),
            new PropertyMetadata(default(double)));

    /// <summary>Минимальный размер цели</summary>
    [Category("WPRGridSplitterEx")]
    [Description("Минимальный размер цели")]
    public double TargetMinSize
    {
        get => (double)GetValue(TargetMinSizeProperty);
        set => SetValue(TargetMinSizeProperty, value);
    }

    #endregion


    /// <summary>Расположение элемента относительно целевого объекта</summary>
    public Placements Placement { get; set; } = Placements.Left;

    private double _Delta;
    private FrameworkElement? _Parent;
    private double _TargetSize;
    private bool _IsCaptured;

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        if(e.LeftButton != MouseButtonState.Pressed) return;

        _Parent = this.FindVisualParent<Window>();
        if (_Parent == null) return;

        switch (Placement)
        {
            case Placements.Left:
            case Placements.Right:
                _Delta = e.GetPosition(_Parent).X;
                _TargetSize = Tagret.ActualWidth;
                break;
            case Placements.Top:
            case Placements.Bottom:
                _Delta = e.GetPosition(_Parent).Y;
                _TargetSize = Tagret.ActualHeight;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        _IsCaptured = CaptureMouse();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!_IsCaptured) return;

        double delta;
        double position;
        var targetMinSize = TargetMinSize;
        switch (Placement)
        {
            case Placements.Left:
                position = e.GetPosition(_Parent).X;
                delta = position - _Delta;
                
                Tagret.Width = Math.Max(_TargetSize - delta, targetMinSize);
                break;

            case Placements.Top:
                position = e.GetPosition(_Parent).Y;
                delta = position - _Delta;
                Tagret.Height = Math.Max(_TargetSize - delta, targetMinSize);
                break;
            case Placements.Right:
                position = e.GetPosition(_Parent).X;
                delta = position - _Delta;
                Tagret.Width = Math.Max(_TargetSize + delta, targetMinSize);
                break;

            case Placements.Bottom:
                position = e.GetPosition(_Parent).Y;
                delta = position - _Delta;
                Tagret.Height = Math.Max(_TargetSize + delta, targetMinSize);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        ReleaseMouseCapture();
        _IsCaptured = false;
        _Parent = null;
    }
}