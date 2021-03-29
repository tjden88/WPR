using System;
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
        private readonly Storyboard _rippleAnimation = new() { DecelerationRatio = 0.5 };
        private Ellipse _ellipse;

        private const double OverSize = 2.0;
        static Ripple()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ripple), new FrameworkPropertyMetadata(typeof(Ripple)));
        }
        public Ripple()
        {
            // Подготовка анимации
            _rippleAnimation.Children.Add(new DoubleAnimation(0, 0, TimeSpan.FromSeconds(0.3)));
            _rippleAnimation.Children.Add(new ThicknessAnimation() { Duration = TimeSpan.FromSeconds(0.3) });
            _rippleAnimation.Children.Add(new DoubleAnimation(1, 0.0, TimeSpan.FromSeconds(0.3)));
            _rippleAnimation.Children.Add(new DoubleAnimation(0, TimeSpan.Zero) { BeginTime = TimeSpan.FromSeconds(0.3) });

            Storyboard.SetTargetProperty(_rippleAnimation.Children[0], new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(_rippleAnimation.Children[1], new PropertyPath(MarginProperty));
            Storyboard.SetTargetProperty(_rippleAnimation.Children[2], new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(_rippleAnimation.Children[3], new PropertyPath(WidthProperty));

            _rippleAnimation.Completed += (o,e) => IsAnimationActive = false;
            MouseDown += (o, e) => _rippleAnimation.SetSpeedRatio(_ellipse, RippleMouseDownSpeed);
            MouseUp += (o, e) => _rippleAnimation.SetSpeedRatio(_ellipse, RippleSpeed);
            MouseLeave += (o, e) => _rippleAnimation.SetSpeedRatio(_ellipse, RippleSpeed);
        }
        

        #region Prop
        /// <summary>
        /// Старт в центре родителя
        /// </summary>
        public bool RippleInCenter
        {
            get => (bool)GetValue(RippleInCenterProperty);
            set => SetValue(RippleInCenterProperty, value);
        }

        public static readonly DependencyProperty RippleInCenterProperty =
            DependencyProperty.Register("RippleInCenter", typeof(bool), typeof(Ripple), new PropertyMetadata(false));

        /// <summary>
        /// Отображается ли анимация в текущий момент
        /// </summary>
        public bool IsAnimationActive
        {
            get => (bool)GetValue(IsAnimationActiveProperty);
            set => SetValue(IsAnimationActiveProperty, value);
        }

        public static readonly DependencyProperty IsAnimationActiveProperty =
            DependencyProperty.Register("IsAnimationActive", typeof(bool), typeof(Ripple), new PropertyMetadata(false));

        /// <summary>
        /// Если False - анимацию нужно запускать вручную
        /// </summary>
        public bool StartRippleOnClick
        {
            get => (bool)GetValue(StartRippleOnClickProperty);
            set => SetValue(StartRippleOnClickProperty, value);
        }

        public static readonly DependencyProperty StartRippleOnClickProperty =
            DependencyProperty.Register("StartRippleOnClick", typeof(bool), typeof(Ripple), new PropertyMetadata(true));


        /// <summary>
        /// Скорость анимации
        /// </summary>
        public double RippleSpeed
        {
            get => (double)GetValue(RippleSpeedProperty);
            set => SetValue(RippleSpeedProperty, value);
        }

        // Using a DependencyProperty as the backing store for RippleSpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RippleSpeedProperty =
            DependencyProperty.Register("RippleSpeed", typeof(double), typeof(Ripple), new PropertyMetadata(1.0, OnRippleSpeedPropertyChanged));

        private static void OnRippleSpeedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Ripple ripple && ripple._ellipse != null) ripple._rippleAnimation.SetSpeedRatio(ripple._ellipse, (double)e.NewValue);
        }


        /// <summary>
        /// Скорость анимации при нажатой кнопке мыши
        /// </summary>
        public double RippleMouseDownSpeed
        {
            get => (double)GetValue(RippleMouseDownSpeedProperty);
            set => SetValue(RippleMouseDownSpeedProperty, value);
        }

        // Using a DependencyProperty as the backing store for RippleMouseDownSpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RippleMouseDownSpeedProperty =
            DependencyProperty.Register("RippleMouseDownSpeed", typeof(double), typeof(Ripple), new PropertyMetadata(1.0));



        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _ellipse = Template.FindName("PART_ellipse", this) as Ellipse;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (StartRippleOnClick) StartRipple(e.GetPosition(this));
            base.OnPreviewMouseLeftButtonDown(e);
        }


        public void StartRipple(Point e = new Point())
        {
            if (_rippleAnimation.Children.Count < 2) return;


            _rippleAnimation.Stop(_ellipse);
            // RippleAnimation.SpeedRatio = RippleMouseDownSpeed;
            var targetWidth = Math.Max(ActualWidth, ActualHeight) * OverSize;
            ((DoubleAnimation) _rippleAnimation.Children[0]).To = targetWidth;

            if (RippleInCenter)
            {
                var position = new Point(ActualWidth / 2, ActualHeight / 2);
                var startMargin = new Thickness(position.X, position.Y, 0, 0);
                _ellipse.Margin = startMargin;
                ((ThicknessAnimation) _rippleAnimation.Children[1]).From = startMargin;
                ((ThicknessAnimation) _rippleAnimation.Children[1]).To = new Thickness(position.X - targetWidth / 2, position.Y - targetWidth / 2, 0, 0);
            }
            else
            {
                var mousePosition = (e);
                var startMargin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);
                _ellipse.Margin = startMargin;
                ((ThicknessAnimation) _rippleAnimation.Children[1]).From = startMargin;
                ((ThicknessAnimation) _rippleAnimation.Children[1]).To = new Thickness(mousePosition.X - targetWidth / 2, mousePosition.Y - targetWidth / 2, 0, 0);
            }
            _rippleAnimation.Begin(_ellipse, true);
            IsAnimationActive = true;
        }
    }
}
