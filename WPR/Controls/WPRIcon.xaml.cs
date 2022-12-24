using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPR.Infrastructure.Icons;

namespace WPR.Controls;

/// <summary> Иконка (векторная) </summary>
public class WPRIcon : Control
{
    private static readonly Lazy<IDictionary<PackIconKind, string>> DataIndex = new(PackIconDataFactory.Create);

    static WPRIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRIcon), new FrameworkPropertyMetadata(typeof(WPRIcon)));
    }

    #region Source
    /// <summary>Значок</summary>
    public PackIconKind Source
    {
        get => (PackIconKind)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    public static readonly DependencyProperty SourceProperty
        = DependencyProperty.Register(nameof(Source), typeof(PackIconKind),
            typeof(WPRIcon), new PropertyMetadata(PackIconKind.InfoCircle,
                (d, e) => ((WPRIcon)d).UpdateData()));
    #endregion


    /// <summary>Размер иконки</summary>
    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register("IconSize", typeof(double), typeof(WPRIcon), new PropertyMetadata(16.0));


    private static readonly DependencyPropertyKey DataPropertyKey
        = DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(WPRIcon), new PropertyMetadata(""));

    public static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the icon path data for the current <see cref="Source"/>.
    /// </summary>
    [TypeConverter(typeof(GeometryConverter))]
    public string Data
    {
        get => (string)GetValue(DataProperty);
        private set => SetValue(DataPropertyKey, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateData();
    }

    private void UpdateData()
    {
        string data = null;
        DataIndex.Value?.TryGetValue(Source, out data);
        Data = data;
    }
}