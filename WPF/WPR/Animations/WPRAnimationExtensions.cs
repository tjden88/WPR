using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace WPR.Animations
{
    /// <summary>
    /// Методы расширения для управления анимациями WPF
    /// </summary>
    public static class WPRAnimationExtensions
    {
        ///// <summary> Добавить анимацию в коллекцию </summary>
        //public static WPRAnimation<T> AddChildren<T>(this WPRAnimation<T> a, AnimationTimeline animation) where T : DependencyObject
        //{ 
        //    a.Storyboard.Children.Add(animation);
        //    return a;
        //}

        /// <summary> Добавить анимацию в коллекцию </summary>
        public static WPRAnimation AddChildren(this WPRAnimation a, AnimationTimeline animation, string PropertyPath)
        {
            Storyboard.SetTarget(animation, a.Target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyPath));

            a.Animation.Children.Add(animation);
            return a;
        }


        /// <summary> Добавить Double анимацию в коллекцию </summary>
        public static WPRAnimation AddDoubleAnimation(this WPRAnimation a, double From, double To, TimeSpan Duration,
            string PropertyPath) => AddChildren(a, new DoubleAnimation(From, To, Duration), PropertyPath);


        /// <summary> Действие при завершении анимации </summary>
        public static WPRAnimation OnComplete(this WPRAnimation a, Action Action)
        {
            a.OnCompleted += Action;
            return a;
        }


        /// <summary> Запустить анимацию </summary>
        public static void Begin(this WPRAnimation a) => a.Animation.Begin();
    }
}
