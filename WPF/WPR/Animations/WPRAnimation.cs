using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace WPR.Animations
{
    /// <summary>
    /// Создание и управление анимациями WPF
    /// </summary>
    public class WPRAnimation//<T> where T : DependencyObject
    {
        public readonly DependencyObject Target;

        public WPRAnimation(DependencyObject Target)
        {
            this.Target = Target;
            Animation.Completed += (_, _) => OnCompleted?.Invoke();
        }


        public Storyboard Animation { get; set; } = new();


        public Action OnCompleted { get; set; }


    }
}
