namespace WPR.Theme;

/// <summary>
/// Типы цветовой темы
/// </summary>
public enum ColorScheme
{
    /// <summary> Светлая тема </summary>
    Light,

    /// <summary> Тёмная тема </summary>
    Dark,

    /// <summary> Автоматическая тема </summary>
    Auto
}


/// <summary>
/// Коллекция кистей цветовой темы
/// </summary>
public enum StyleBrushes
{
    None,
    TransparentBrush,
    PrimaryColorBrush,
    DarkPrimaryColorBrush,
    LightPrimaryColorBrush,
    AccentColorBrush,
    DangerColorBrush,
    SecondaryColorBrush,
    TextColorBrush,
    DividerColorBrush,
    BackgroundColorBrush,
    SecondaryBackgroundColorBrush,
    WindowTitleBackgroundColorBrush,
    AnimationEnterColorBrush,
    BackgroundContrastColorBrush,
    WindowTitleBackgroundContrastColorBrush,
    PrimaryContrastColorBrush,
    AccentContrastColorBrush,
    SuccessColorBrush
}