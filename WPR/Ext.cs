using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace WPR;

/// <summary>
/// Присоединённые свойства для расширения отображения и поведения контролов
/// </summary>
public static class Ext
{
    #region Радиус бордера элементов
    /// <summary>
    /// Радиус бордера элементов
    /// </summary>
    public static readonly DependencyProperty BorderRadiusProperty =
        DependencyProperty.RegisterAttached("BorderRadius", typeof(CornerRadius), typeof(Ext), new PropertyMetadata(default(CornerRadius)));

    public static void SetBorderRadius(UIElement element, CornerRadius value) => element.SetValue(BorderRadiusProperty, value);

    public static CornerRadius GetBorderRadius(UIElement element) => (CornerRadius)element.GetValue(BorderRadiusProperty);

    #endregion

    #region Attached property MouseOverBrush : Brush - Кисть заливки при наведении мыши

    /// <summary>Кисть заливки при наведении мыши</summary>
    public static readonly DependencyProperty MouseOverBrushProperty =
        DependencyProperty.RegisterAttached(
            "MouseOverBrush",
            typeof(Brush),
            typeof(Ext),
            new PropertyMetadata(default(Brush)));

    /// <summary>Кисть заливки при наведении мыши</summary>
    public static void SetMouseOverBrush(DependencyObject d, Brush value) => d.SetValue(MouseOverBrushProperty, value);

    /// <summary>Кисть заливки при наведении мыши</summary>
    public static Brush GetMouseOverBrush(DependencyObject d) => (Brush)d.GetValue(MouseOverBrushProperty);

    #endregion

    #region Attached property Shadow : DropShadowEffect - Тень элемента

    /// <summary>Тень элемента</summary>
    public static readonly DependencyProperty ShadowProperty =
        DependencyProperty.RegisterAttached(
            "Shadow",
            typeof(DropShadowEffect),
            typeof(Ext),
            new PropertyMetadata(default(DropShadowEffect)));

    /// <summary>Тень элемента</summary>
    public static void SetShadow(DependencyObject d, DropShadowEffect value) => d.SetValue(ShadowProperty, value);

    /// <summary>Тень элемента</summary>
    public static DropShadowEffect GetShadow(DependencyObject d) => (DropShadowEffect)d.GetValue(ShadowProperty);

    #endregion

    #region Подсказка для текстовых полей или комбобоксов
    /// <summary>
    /// Подсказка для текстовых полей или комбобоксов
    /// </summary>
    public static readonly DependencyProperty TextHintProperty =
        DependencyProperty.RegisterAttached("TextHint", typeof(object), typeof(Ext), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static void SetTextHint(UIElement element, object value) => element?.SetValue(TextHintProperty, value);

    public static object GetTextHint(UIElement element) => element?.GetValue(TextHintProperty);

    #endregion

    #region Дополнительное описание для текстовых полей или комбобоксов
    /// <summary>
    /// Дополнительное описание для текстовых полей или комбобоксов
    /// </summary>
    public static readonly DependencyProperty TextDescriptionProperty =
        DependencyProperty.RegisterAttached("TextDescription", typeof(object), typeof(Ext),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static void SetTextDescription(UIElement element, object value) => element.SetValue(TextDescriptionProperty, value);

    public static object GetTextDescription(UIElement element) => (object)element.GetValue(TextDescriptionProperty);

    #endregion

    #region Attached property ClearButton : bool - Показать ли кнопку очистки текстбокса или комбобокса

    /// <summary>Показать ли кнопку очистки текстбокса или комбобокса</summary>
    public static readonly DependencyProperty ClearButtonProperty =
        DependencyProperty.RegisterAttached(
            "ClearButton",
            typeof(bool),
            typeof(Ext),
            new PropertyMetadata(false));

    /// <summary>Показать ли кнопку очистки текстбокса или комбобокса</summary>
    public static void SetClearButton(DependencyObject d, bool value) => d.SetValue(ClearButtonProperty, value);

    /// <summary>Показать ли кнопку очистки текстбокса или комбобокса</summary>
    public static bool GetClearButton(DependencyObject d) => (bool)d.GetValue(ClearButtonProperty);

    #endregion

    #region Контент заголовка окна (слева в строке заголовка)
    public static object GetWindowHeaderContent(DependencyObject obj)
    {
        return obj.GetValue(WindowHeaderContentProperty);
    }

    public static void SetWindowHeaderContent(DependencyObject obj, object value)
    {
        obj.SetValue(WindowHeaderContentProperty, value);
    }

    /// <summary> Контент заголовка окна (слева в строке заголовка) </summary>
    public static readonly DependencyProperty WindowHeaderContentProperty =
        DependencyProperty.RegisterAttached("WindowHeaderContent", typeof(object), typeof(Ext), new PropertyMetadata(null));
    #endregion



    #region Internal

    #region Запретить Ripple выходить за границы кнопки

    /// <summary>
    /// Запретить Ripple выходить за границы кнопки
    /// </summary>
    internal static bool GetNoRippleCircleOutside(DependencyObject obj) => (bool)obj.GetValue(NoRippleCircleOutsideProperty);

    internal static void SetNoRippleCircleOutside(DependencyObject obj, bool value) => obj.SetValue(NoRippleCircleOutsideProperty, value);

    internal static readonly DependencyProperty NoRippleCircleOutsideProperty =
        DependencyProperty.RegisterAttached("NoRippleCircleOutside", typeof(bool), typeof(Ext), new PropertyMetadata(true));
    #endregion

    #endregion
}