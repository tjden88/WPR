using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPR.Styles.Controls
{
    /// <summary>
    /// Анимация кнопок и прочих контролов
    /// </summary>
    public class Ripple : ContentControl
    {
        private readonly Storyboard RippleAnimation = new Storyboard() { DecelerationRatio = 0.5 };
        private Ellipse ellipse;

        private const double OverSize = 2.0;
        static Ripple()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ripple), new FrameworkPropertyMetadata(typeof(Ripple)));
        }
        public Ripple()
        {
            // Подготовка анимации
            RippleAnimation.Children.Add(new DoubleAnimation(0, 0, TimeSpan.FromSeconds(0.3)));
            RippleAnimation.Children.Add(new ThicknessAnimation() { Duration = TimeSpan.FromSeconds(0.3) });
            RippleAnimation.Children.Add(new DoubleAnimation(1, 0.0, TimeSpan.FromSeconds(0.3)));
            RippleAnimation.Children.Add(new DoubleAnimation(0, TimeSpan.Zero) { BeginTime = TimeSpan.FromSeconds(0.3) });

            Storyboard.SetTargetProperty(RippleAnimation.Children[0], new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(RippleAnimation.Children[1], new PropertyPath(MarginProperty));
            Storyboard.SetTargetProperty(RippleAnimation.Children[2], new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(RippleAnimation.Children[3], new PropertyPath(WidthProperty));

            RippleAnimation.Completed += RippleAnimation_Completed;
            MouseDown += Ripple_MouseDown;
            MouseUp += Ripple_MouseUp;
            MouseLeave += Ripple_MouseLeave;
        }

        private void Ripple_MouseLeave(object sender, MouseEventArgs e)
        {
            RippleAnimation.SetSpeedRatio(ellipse, RippleSpeed);
        }

        private void Ripple_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RippleAnimation.SetSpeedRatio(ellipse, RippleSpeed);
        }

        private void Ripple_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RippleAnimation.SetSpeedRatio(ellipse, RippleMouseDownSpeed);
        }

        private void RippleAnimation_Completed(object sender, EventArgs e)
        {
            IsAnimationActive = false;
        }


        #region Prop
        /// <summary>
        /// Старт в центре родителя
        /// </summary>
        public bool RippleInCenter
        {
            get { return (bool)GetValue(RippleInCenterProperty); }
            set { SetValue(RippleInCenterProperty, value); }
        }

        public static readonly DependencyProperty RippleInCenterProperty =
            DependencyProperty.Register("RippleInCenter", typeof(bool), typeof(Ripple), new PropertyMetadata(false));

        /// <summary>
        /// Отображается ли анимация в текущий момент
        /// </summary>
        public bool IsAnimationActive
        {
            get { return (bool)GetValue(IsAnimationActiveProperty); }
            set { SetValue(IsAnimationActiveProperty, value); }
        }

        public static readonly DependencyProperty IsAnimationActiveProperty =
            DependencyProperty.Register("IsAnimationActive", typeof(bool), typeof(Ripple), new PropertyMetadata(false));

        /// <summary>
        /// Если False - анимацию нужно запускать вручную
        /// </summary>
        public bool StartRippleOnClick
        {
            get { return (bool)GetValue(StartRippleOnClickProperty); }
            set { SetValue(StartRippleOnClickProperty, value); }
        }

        public static readonly DependencyProperty StartRippleOnClickProperty =
            DependencyProperty.Register("StartRippleOnClick", typeof(bool), typeof(Ripple), new PropertyMetadata(true));


        /// <summary>
        /// Скорость анимации
        /// </summary>
        public double RippleSpeed
        {
            get { return (double)GetValue(RippleSpeedProperty); }
            set
            {
                SetValue(RippleSpeedProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for RippleSpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RippleSpeedProperty =
            DependencyProperty.Register("RippleSpeed", typeof(double), typeof(Ripple), new PropertyMetadata(1.0, new PropertyChangedCallback(AAA)));

        private static void AAA(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ripple ripple = d as Ripple;
            if (ripple.ellipse != null) ripple.RippleAnimation.SetSpeedRatio(ripple.ellipse, (double)e.NewValue);
        }


        /// <summary>
        /// Скорость анимации при нажатой кнопке мыши
        /// </summary>
        public double RippleMouseDownSpeed
        {
            get { return (double)GetValue(RippleMouseDownSpeedProperty); }
            set { SetValue(RippleMouseDownSpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RippleMouseDownSpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RippleMouseDownSpeedProperty =
            DependencyProperty.Register("RippleMouseDownSpeed", typeof(double), typeof(Ripple), new PropertyMetadata(1.0));



        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ellipse = Template.FindName("PART_ellipse", this) as Ellipse;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (StartRippleOnClick) StartRipple(e.GetPosition(this));
            base.OnPreviewMouseLeftButtonDown(e);
        }


        public void StartRipple(Point e = new Point())
        {
            RippleAnimation.Stop(ellipse);
            // RippleAnimation.SpeedRatio = RippleMouseDownSpeed;
            var targetWidth = Math.Max(ActualWidth, ActualHeight) * OverSize;
            (RippleAnimation.Children[0] as DoubleAnimation).To = targetWidth;

            if (RippleInCenter)
            {
                var Position = new Point(ActualWidth / 2, ActualHeight / 2);
                var startMargin = new Thickness(Position.X, Position.Y, 0, 0);
                ellipse.Margin = startMargin;
                (RippleAnimation.Children[1] as ThicknessAnimation).From = startMargin;
                (RippleAnimation.Children[1] as ThicknessAnimation).To = new Thickness(Position.X - targetWidth / 2, Position.Y - targetWidth / 2, 0, 0);
            }
            else
            {
                var mousePosition = (e);
                var startMargin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);
                ellipse.Margin = startMargin;
                (RippleAnimation.Children[1] as ThicknessAnimation).From = startMargin;
                (RippleAnimation.Children[1] as ThicknessAnimation).To = new Thickness(mousePosition.X - targetWidth / 2, mousePosition.Y - targetWidth / 2, 0, 0);
            }
            RippleAnimation.Begin(ellipse, true);
            IsAnimationActive = true;
        }
    }
}
