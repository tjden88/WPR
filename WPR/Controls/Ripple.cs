using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPR.Controls
{
    /// <summary> Анимация кнопок и прочих контролов </summary>
    public class Ripple : ContentControl
    {
        private readonly Storyboard _RippleAnimation = new() { DecelerationRatio = 0.5 };
        private Ellipse _Ellipse;

        private const double OverSize = 2.0;
        static Ripple()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ripple), new FrameworkPropertyMetadata(typeof(Ripple)));
        }
        public Ripple()
        {
            // Подготовка анимации
            _RippleAnimation.Children.Add(new DoubleAnimation(0, 0, TimeSpan.FromSeconds(0.3)));
            _RippleAnimation.Children.Add(new ThicknessAnimation() { Duration = TimeSpan.FromSeconds(0.3) });
            _RippleAnimation.Children.Add(new DoubleAnimation(1, 0.0, TimeSpan.FromSeconds(0.3)));
            _RippleAnimation.Children.Add(new DoubleAnimation(0, TimeSpan.Zero) { BeginTime = TimeSpan.FromSeconds(0.3) });

            Storyboard.SetTargetProperty(_RippleAnimation.Children[0], new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(_RippleAnimation.Children[1], new PropertyPath(MarginProperty));
            Storyboard.SetTargetProperty(_RippleAnimation.Children[2], new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(_RippleAnimation.Children[3], new PropertyPath(WidthProperty));

            _RippleAnimation.Completed += (o,e) => IsAnimationActive = false;
            MouseDown += (o, e) => _RippleAnimation.SetSpeedRatio(_Ellipse, RippleMouseDownSpeed);
            MouseUp += (o, e) => _RippleAnimation.SetSpeedRatio(_Ellipse, RippleSpeed);
            MouseLeave += (o, e) => _RippleAnimation.SetSpeedRatio(_Ellipse, RippleSpeed);
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
            if (d is Ripple ripple && ripple._Ellipse != null) ripple._RippleAnimation.SetSpeedRatio(ripple._Ellipse, (double)e.NewValue);
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
            _Ellipse = Template.FindName("PART_ellipse", this) as Ellipse;
            if (_Ellipse == null) throw new NullReferenceException("Эллипс в шаблоне не найден!");
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (StartRippleOnClick) StartRipple(e.GetPosition(this));
            base.OnPreviewMouseLeftButtonDown(e);
        }


        public void StartRipple(Point e = new Point())
        {
            if (_RippleAnimation.Children.Count < 2) return;


            _RippleAnimation.Stop(_Ellipse);
            var targetWidth = Math.Max(ActualWidth, ActualHeight) * OverSize;
            ((DoubleAnimation) _RippleAnimation.Children[0]).To = targetWidth;

            if (RippleInCenter)
            {
                var position = new Point(ActualWidth / 2, ActualHeight / 2);
                var startMargin = new Thickness(position.X, position.Y, 0, 0);
                _Ellipse.Margin = startMargin;
                ((ThicknessAnimation) _RippleAnimation.Children[1]).From = startMargin;
                ((ThicknessAnimation) _RippleAnimation.Children[1]).To = new Thickness(position.X - targetWidth / 2, position.Y - targetWidth / 2, 0, 0);
            }
            else
            {
                var mousePosition = (e);
                var startMargin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);
                _Ellipse.Margin = startMargin;
                ((ThicknessAnimation) _RippleAnimation.Children[1]).From = startMargin;
                ((ThicknessAnimation) _RippleAnimation.Children[1]).To = new Thickness(mousePosition.X - targetWidth / 2, mousePosition.Y - targetWidth / 2, 0, 0);
            }
            _RippleAnimation.Begin(_Ellipse, true);
            IsAnimationActive = true;
        }
    }
}
