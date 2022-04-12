using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WPR.Controls
{
    public class WPRPopup : Popup
    {
        private readonly Storyboard _ShowAnimation, _HideAnimation;
        private bool _StaysOpenIsChangeg; //Определить, изменили ли временно свойство для закрытия с анимацией
        private readonly Grid _RootGrid = new() { Background = Brushes.Transparent };
        private readonly WPRCard _RootCard = new() { IsPopupShadowStyle = true };
        private readonly Thumb _Thumb = new() { Width = 0, Height = 0 };

        #region Properties
        /// <summary> Контент Попапа. 
        /// Использовать это свойство зависимостей, НЕ ПЕРЕОПРЕДЕЛЯТЬ свойство CHild!
        /// </summary>
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        [Category("WPRPopup")]
        [Description("Контент Попапа")]

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(WPRPopup), new PropertyMetadata(null));



        /// <summary> Разрешить перетаскивание мышкой </summary>
        public bool AllowMouseMove
        {
            get => (bool)GetValue(AllowMouseMoveProperty);
            set => SetValue(AllowMouseMoveProperty, value);
        }

        [Category("WPRPopup")]
        [Description("Разрешить перетаскивание мышкой")]
        public static readonly DependencyProperty AllowMouseMoveProperty =
            DependencyProperty.Register("AllowMouseMove", typeof(bool), typeof(WPRPopup), new PropertyMetadata(false));

        #region CloseOnMouseButtonUp : bool - Закрывать при клике внутри области попапа

        /// <summary>Закрывать при клике внутри области попапа</summary>
        public static readonly DependencyProperty CloseOnMouseButtonUpProperty =
            DependencyProperty.Register(
                nameof(CloseOnMouseButtonUp),
                typeof(bool),
                typeof(WPRPopup),
                new PropertyMetadata(default(bool)));

        /// <summary>Закрывать при клике внутри области попапа</summary>
        [Category("WPRPopup")]
        [Description("Закрывать при клике внутри области попапа")]
        public bool CloseOnMouseButtonUp
        {
            get => (bool)GetValue(CloseOnMouseButtonUpProperty);
            set => SetValue(CloseOnMouseButtonUpProperty, value);
        }

        #endregion

        #endregion

        public WPRPopup()
        {
            PopupAnimation = PopupAnimation.None;
            AllowsTransparency = true;

            // Определение разметки
            Child = _RootGrid;

            ScaleTransform scale = new(0, 0);

            _RootGrid.RenderTransform = scale;
            _RootGrid.SetBinding(RenderTransformOriginProperty, new Binding("RenderTransformOrigin") { Source = this });


            _RootCard.SetBinding(ContentControl.ContentProperty, new Binding("Content") { Source = this });
            _RootCard.SetBinding(RenderTransformOriginProperty, new Binding("RenderTransformOrigin") { Source = this });
            _RootCard.SetBinding(RenderTransformProperty, new Binding("RenderTransform") { Source = this });
            _RootCard.SetBinding(LayoutTransformProperty, new Binding("LayoutTransform") { Source = this });

            _RootGrid.Children.Add(_Thumb);
            _RootGrid.Children.Add(_RootCard);

            PreviewMouseUp += OnMouseUp;


            // Реализация перетаскивания контента

            _Thumb.DragDelta += (sender, e) =>
            {
                HorizontalOffset += e.HorizontalChange;
                VerticalOffset += e.VerticalChange;
            };

            // Подготовка анимации
            CircleEase easein = new() { EasingMode = EasingMode.EaseIn };
            ElasticEase easeout = new() { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 8 };

            _ShowAnimation = new Storyboard();
            _ShowAnimation.Children.Add(new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.4)) { EasingFunction = easeout });
            _ShowAnimation.Children.Add(new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.4)) { EasingFunction = easeout });

            Storyboard.SetTargetProperty(_ShowAnimation.Children[0], new PropertyPath("(RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(_ShowAnimation.Children[1], new PropertyPath("(RenderTransform).(ScaleTransform.ScaleY)"));

            _HideAnimation = new Storyboard();
            _HideAnimation.Children.Add(new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2)) { EasingFunction = easein });
            _HideAnimation.Children.Add(new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2)) { EasingFunction = easein });

            Storyboard.SetTargetProperty(_HideAnimation.Children[0], new PropertyPath("(RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(_HideAnimation.Children[1], new PropertyPath("(RenderTransform).(ScaleTransform.ScaleY)"));

            _HideAnimation.Completed += delegate
            {
                if (_StaysOpenIsChangeg)
                {
                    StaysOpen = false;
                }
                IsOpen = false;
            };

            Opened += WPRPopup_Opened;

        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            _Thumb.RaiseEvent(e);
        }

        private void OnMouseUp(object Sender, MouseButtonEventArgs E)
        {
            if (CloseOnMouseButtonUp) Hide();
        }

        private async void WPRPopup_Opened(object sender, EventArgs e)
        {
            if (Content == null) return;


            HorizontalOffset = 0;
            VerticalOffset = 0;
            _RootGrid.IsEnabled = true;
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                _ShowAnimation.Begin(_RootGrid);
            }), DispatcherPriority.Background);
        }

        /// <summary>
        /// Метод позволяет вручную закрыть Popup с анимацией
        /// </summary>
        public void Hide()
        {
            if (Content != null)
            {
                _RootGrid.IsEnabled = false;
                Dispatcher.BeginInvoke(new Action(() => _HideAnimation.Begin(_RootGrid)), DispatcherPriority.Background);
            }
            else
            {
                IsOpen = false;
            }
        }

        /// <summary>
        /// Показать окно
        /// </summary>
        public void Show() => IsOpen = true;

        /// <summary>
        /// Скрыть окно
        /// </summary>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (!StaysOpen) // Задержать закрытие до окончания анимации
            {
                if (Content != null)
                {
                    Point target = e.GetPosition(_RootCard);
                    if (target.X < 0 || target.Y < 0 || target.X > _RootCard.ActualWidth || target.Y > _RootCard.ActualHeight)
                    {
                        _StaysOpenIsChangeg = true;
                        StaysOpen = true;
                        Hide();
                    }
                }
            }
            base.OnPreviewMouseDown(e);
        }

    }
}
