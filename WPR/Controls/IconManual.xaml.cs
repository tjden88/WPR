using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPR;

/// <summary>
/// Иконка, построенная из <see cref="Geometry"/>, в стиле Material-паков:
/// рисуется как <see cref="Path"/> с заливкой от <see cref="Control.Foreground"/>.
/// </summary>
/// <remarks>
/// - чтобы использовать кастомные иконки без добавления в PackIconDataFactory;
/// </remarks>
public sealed class IconManual : Control
{
    static IconManual()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(IconManual),
            new FrameworkPropertyMetadata(typeof(IconManual)));
    }

    #region Data - Геометрия иконки

    /// <summary>
    /// Геометрия (Path.Data), описывающая иконку.
    /// Обычно задаётся как ресурс <see cref="Geometry"/> или через конвертер из строки.
    /// </summary>
    public Geometry? Data
    {
        get => (Geometry?)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <summary>
    /// DependencyProperty для <see cref="Data"/>.
    /// </summary>
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(
            nameof(Data),
            typeof(Geometry),
            typeof(IconManual),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion

    /// <summary>Размер иконки</summary>
    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(IconManual), new PropertyMetadata(16.0));

}
