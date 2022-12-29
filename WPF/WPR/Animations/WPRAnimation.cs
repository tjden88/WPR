using System.Windows;
using System.Windows.Media.Animation;

namespace WPR.Animations;

/// <summary>
/// Создание и управление анимациями WPF
/// </summary>
public class WPRAnimation//<T> where T : DependencyObject
{
    /// <summary> Целевой объект анимации </summary>
    public readonly FrameworkElement Target;

    public WPRAnimation(FrameworkElement Target)
    {
        this.Target = Target;
        Animation = new();
    }

    /// <summary> Объект анимации </summary>
    public Storyboard Animation { get; set; }

}