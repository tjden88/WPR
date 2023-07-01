using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WPR.Animations;

/// <summary>
/// Методы расширения для управления анимациями WPF
/// </summary>
public static class StoryboardExtensions
{

    /// <summary> Добавить анимацию в коллекцию </summary>
    public static Storyboard AddAnimationTimeline(this Storyboard a, string PropertyPath, AnimationTimeline animation)
    {
        //Storyboard.SetTarget(animation, a.Target);
        Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyPath));

        a.Children.Add(animation);
        return a;
    }


    /// <summary> Добавить Double анимацию в коллекцию </summary>
    public static Storyboard AddDoubleAnimation(this Storyboard a, string PropertyPath,
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
    public static Storyboard OnComplete(this Storyboard a, Action Action)
    {
        a.Completed += (s, _) => Action?.Invoke();
        return a;
    }


    /// <summary> Освободить свойства анимированных элементов при завершениии </summary>
    public static Storyboard ClearOnComplete(this Storyboard a)
    {
        a.FillBehavior = FillBehavior.Stop;
        return a;
    }


    /// <summary> Запустить анимацию для выбранного объекта</summary>
    public static void Start(this Storyboard a, FrameworkElement Target)//, DispatcherPriority Priority = DispatcherPriority.Normal)
    {
        //Target.Dispatcher?.Invoke(Priority, () =>
        //{
        //    a.Begin(Target);
        //});
        a.Begin(Target);
    }


    /// <summary> Найти функцию плавности в ресурсах </summary>
    private static IEasingFunction GetEasingFunction(EasingFunctions easingFunctions) =>
        Application.Current.Resources[easingFunctions.ToString()] as IEasingFunction;
}