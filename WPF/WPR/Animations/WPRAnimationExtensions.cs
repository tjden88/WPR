using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace WPR.Animations;

/// <summary>
/// Методы расширения для управления анимациями WPF
/// </summary>
public static class WPRAnimationExtensions
{
    /// <summary> Добавить анимацию в коллекцию </summary>
    public static WPRAnimation AddAnimationTimeline(this WPRAnimation a, string PropertyPath, AnimationTimeline animation)
    {
        Storyboard.SetTarget(animation, a.Target);
        Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyPath));

        a.Animation.Children.Add(animation);
        return a;
    }


    /// <summary> Добавить Double анимацию в коллекцию </summary>
    public static WPRAnimation AddDoubleAnimation(this WPRAnimation a, string PropertyPath,
        double From=0,
        double To=1,
        double MsDuration=300,
        EasingFunctions EasingFunction= EasingFunctions.None)
        => AddAnimationTimeline(a, PropertyPath, 
            new DoubleAnimation(From, To, TimeSpan.FromMilliseconds(MsDuration))
            {
                EasingFunction = GetEasingFunction(EasingFunction)
            });



    /// <summary> Действие при завершении анимации </summary>
    public static WPRAnimation OnComplete(this WPRAnimation a, Action Action)
    {
        a.Animation.Completed += (s, _) => Action?.Invoke();
        return a;
    }


    /// <summary> Освободить свойства анимированных элементов при завершениии </summary>
    public static WPRAnimation ClearOnComplete(this WPRAnimation a)
    {
        a.Animation.FillBehavior = FillBehavior.Stop;
        return a;
    }


    /// <summary> Запустить анимацию </summary>
    public static void Begin(this WPRAnimation a) => a.Animation.Begin();


    /// <summary> Найти функцию плавности в ресурсах </summary>
    private static IEasingFunction GetEasingFunction(EasingFunctions easingFunctions) =>
        Application.Current.Resources[easingFunctions.ToString()] as IEasingFunction;
}