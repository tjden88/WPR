using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPR.Controls
{
    public class WPRPopup : Popup
    {
        private readonly Storyboard _ShowAnimation, _HideAnimation;
        private bool _StaysOpenIsChangeg; //Определить, изменили ли временно свойство для закрытия с анимацией
        private readonly WPRCard _RootCard;

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
            get => (bool) GetValue(CloseOnMouseButtonUpProperty);
            set => SetValue(CloseOnMouseButtonUpProperty, value);
        }

        #endregion

        #endregion

        public WPRPopup()
        {
            PopupAnimation = PopupAnimation.None;
            AllowsTransparency = true;

            // Определение разметки
            Grid grid = new();
            Child = grid;

            var thumb = new Thumb
            {
                Width = 0,
                Height = 0,
            };

            ScaleTransform scale = new(0, 0);

            _RootCard = new()
            {
                RenderTransform = scale,
                IsPopupShadowStyle = true
            };
            _RootCard.SetBinding(ContentControl.ContentProperty, new Binding("Content") { Source = this });
            _RootCard.SetBinding(RenderTransformOriginProperty, new Binding("RenderTransformOrigin") { Source = this });

            grid.Children.Add(thumb);
            grid.Children.Add(_RootCard);

            PreviewMouseUp += OnMouseUp;


            // Реализация перетаскивания контента
            MouseDown += (sender, e) =>
            {
                if (AllowMouseMove) thumb.RaiseEvent(e);
            };

            thumb.DragDelta += (sender, e) =>
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

            Opened += PRExPopup_Opened;

        }

        private void OnMouseUp(object Sender, MouseButtonEventArgs E)
        {
            if(CloseOnMouseButtonUp) Hide();
        }

        private void PRExPopup_Opened(object sender, EventArgs e)
        {
            if (Content != null)
            {
                HorizontalOffset = 0;
                VerticalOffset = 0;
                _RootCard.IsEnabled = true;
                _ShowAnimation.Begin(_RootCard);
            }
        }

        /// <summary>
        /// Метод позволяет вручную закрыть Popup с анимацией
        /// </summary>
        public void Hide()
        {
            if (Content != null)
            {
                _RootCard.IsEnabled = false;
                _HideAnimation.Begin(_RootCard);
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
