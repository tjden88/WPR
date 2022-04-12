﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPR.Controls
{
    /// <summary> Контрол для обёртки диалоговых окон </summary>
    internal class WPRDialogPanel : HeaderedContentControl
    {
        private IWPRDialog _WPRDialog;
        private bool _StaysOpen;

        private WPRPopup _HeaderPopup;

        static WPRDialogPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRDialogPanel), new FrameworkPropertyMetadata(typeof(WPRDialogPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("BubbleButton") is Button b) b.Click += BubbleButton_Click;
            if (GetTemplateChild("PART_Bubble") is Border br) br.MouseUp += (_, _) => HideBubble();
            if (GetTemplateChild("PART_Rect") is Rectangle r) r.MouseDown += Rect_MouseDown;

            _HeaderPopup = GetTemplateChild("PART_Popup") as WPRPopup;

            if (_HeaderPopup == null)
                throw new ArgumentNullException(nameof(_HeaderPopup), "Попап не найден в шаблоне!");

            _HeaderPopup.PopupClosed += HeaderPopupOnClosed;
            //_HeaderPopup.PopupShowed += () =>
            //{
            //    if (GetTemplateChild("PART_HeaderContent") is ContentPresenter presenter) presenter.Focus();
            //};
        }


        #region Диалоговое окно

        private readonly Queue<(object content, bool staysOpen)> _DialogContentQueue = new(); // Очередь объектов для отображения


        #region IsShowingProperty
        /// <summary> Показан ли какой-либо диалог </summary>
        public bool IsShowing
        {
            get => (bool)GetValue(IsShowingProperty);
            set => SetValue(IsShowingProperty, value);
        }
        public static readonly DependencyProperty IsShowingProperty =
            DependencyProperty.Register("IsShowing", typeof(bool), typeof(WPRDialogPanel), new PropertyMetadata(false));

        #endregion


        /// <summary>
        /// Показать контент и затемнить родителя
        /// </summary>
        /// <param name="content">Объект - содержимое для показа в качестве диалога</param>
        /// <param name="staysOpen">Не позволять закрыть содержимое при клике за его пределы</param>
        public void Show(object content, bool staysOpen)
        {

            // Диалог показан в данный момент
            if (IsShowing)
            {
                // Поместить текущий диалог в очередь и запустить новый
                _DialogContentQueue.Enqueue((Header, _StaysOpen));
                _DialogContentQueue.Enqueue((content, staysOpen));
                Hide();
            }
            else
            {
                _DialogContentQueue.Enqueue((content, staysOpen));
                ShowFromQueue();
            }

        }


        /// <summary>
        /// Скрыть контент и осветлить родителя
        /// </summary>
        public void Hide()
        {
            if (!IsShowing) return;
            _HeaderPopup.Hide();
            _WPRDialog = null;
        }

        // Показать следующий контент
        private void ShowFromQueue()
        {
            if (!_DialogContentQueue.TryDequeue(out var nextContent))
            {
                Header = null;
                Focus();
                return;
            }
            if (nextContent.content is IWPRDialog dlg)
            {
                _WPRDialog = dlg;
                Header = dlg.DialogContent;
            }
            else
            {
                _WPRDialog = null;
                Header = nextContent.content;
            }
            _HeaderPopup.Show();
            _StaysOpen = nextContent.staysOpen;
            IsShowing = true;

        }

        #endregion


        #region Всплывающая подсказка

        #region BubbleText
        /// <summary> Текст всплывающей подсказки </summary>
        public string BubbleText
        {
            get => (string)GetValue(BubbleTextProperty);
            set => SetValue(BubbleTextProperty, value);
        }

        public static readonly DependencyProperty BubbleTextProperty =
            DependencyProperty.Register("BubbleText", typeof(string), typeof(WPRDialogPanel), new PropertyMetadata(""));


        #endregion


        /// <summary>
        /// Показать всплывающую подсказку
        /// </summary>
        /// <param name="Text">Текст сообщения</param>
        /// <param name="Duration">Длительность</param>
        /// <param name="ButtonCommandText">Текст кнопки команды</param>
        /// <param name="Callback">True, если кнопка была нажата</param>
        public void ShowBubble(string Text, int Duration = 2000, string ButtonCommandText = "", Action<bool> Callback = null)
        {
            _StackBubblesQueue.Enqueue(new StackBubbles { Text = Text, Duration = Duration, Buttontext = ButtonCommandText, Action = Callback });
            if (_StackBubblesQueue.Count == 1) ShowBubbleinStack();
        }

        private void HideBubble()
        {
            var clip = GetTemplateChild("PART_Bubble") as Border;
            _Animout.Completed -= Animout_Completed;
            _Animout = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            _Animout.Completed += Animout_Completed;
            clip?.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _Animout);
        }

        private void ShowBubbleinStack()
        {
            if (!_StackBubblesQueue.TryPeek(out var stack)) return;
            BubbleText = stack.Text;

            if (GetTemplateChild("BubbleButton") is Button commandbutton)
            {
                if (!string.IsNullOrEmpty(stack.Buttontext))
                {
                    commandbutton.Content = stack.Buttontext;
                    commandbutton.Visibility = Visibility.Visible;
                }
                else
                {
                    commandbutton.Visibility = Visibility.Collapsed;
                }
            }


            if (GetTemplateChild("PART_Bubble") is not Border clip) return;

            ScaleTransform trans = new(1, 0);
            clip.RenderTransform = trans;
            DoubleAnimation anim = new(1, TimeSpan.FromSeconds(0.1));
            anim.Completed += delegate
            {
                _Animout = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1)) { BeginTime = TimeSpan.FromMilliseconds(stack.Duration) };
                _Animout.Completed += Animout_Completed;
                trans.BeginAnimation(ScaleTransform.ScaleYProperty, _Animout);
            };

            trans.BeginAnimation(ScaleTransform.ScaleYProperty, anim);

        }

        private void BubbleButton_Click(object sender, RoutedEventArgs e)
        {
            _StackBubblesQueue.Peek().Action?.Invoke(true);
            HideBubble();
        }

        private void Animout_Completed(object sender, EventArgs e)
        {
            _StackBubblesQueue.Dequeue().Action?.Invoke(false);
            ShowBubbleinStack();
        }

        private DoubleAnimation _Animout;

        private readonly Queue<StackBubbles> _StackBubblesQueue = new(); // Очередь подсказок
        private struct StackBubbles
        {
            public string Text;
            public int Duration;
            public string Buttontext;
            public Action<bool> Action;
        }
        #endregion


        #region PrivateMethods

        private void HeaderPopupOnClosed()
        {
            IsShowing = false;
            ShowFromQueue();
        }

        // Показать анимацию контента при клике на заблокированную область
        private void Rect_MouseDown(object sender, MouseEventArgs e)
        {
            if (_StaysOpen)
            {
                if (Template.Resources["ShakeAnim"] is Storyboard s)
                {
                    s.Begin(_HeaderPopup);
                }
            }
            else
            {
                _WPRDialog?.DialogResult?.Invoke(false);
                Hide();
            }
        }

        #endregion
    }
}
