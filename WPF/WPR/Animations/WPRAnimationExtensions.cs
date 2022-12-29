using System;
using System.Windows;
using System.Windows.Media;
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
        public static WPRAnimation AddAnimationTimeline(this WPRAnimation a, AnimationTimeline animation, string PropertyPath)
        {
            Storyboard.SetTarget(animation, a.Target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyPath));

            a.Animation.Children.Add(animation);
            return a;
        }


        /// <summary> Добавить Double анимацию в коллекцию </summary>
        public static WPRAnimation AddDoubleAnimation(this WPRAnimation a, double From, double To, TimeSpan Duration, EasingFunctions EasingFunction,
            string PropertyPath) => AddAnimationTimeline(a, new DoubleAnimation(From, To, Duration) {EasingFunction = GetEasingFunction(EasingFunction)}, PropertyPath);


        /// <summary> Действие при завершении анимации </summary>
        public static WPRAnimation OnComplete(this WPRAnimation a, Action Action)
        {
            a.OnCompleted += Action;
            return a;
        }


        /// <summary> Запустить анимацию </summary>
        public static void Begin(this WPRAnimation a) => a.Animation.Begin();

        private static IEasingFunction GetEasingFunction(EasingFunctions easingFunctions) => Application.Current.Resources[easingFunctions.ToString()] as IEasingFunction;
    }
}
