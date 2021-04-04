using System;
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
    public class WPRDialogPanel : HeaderedContentControl
    {
        public event EventHandler ButtonClicked;

        static WPRDialogPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRDialogPanel), new FrameworkPropertyMetadata(typeof(WPRDialogPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("BubbleButton") is Button b) b.Click += BubbleButton_Click;
            if (GetTemplateChild("PART_Bubble") is Border br) br.MouseUp += (_,_) => HideBubble();
            if (GetTemplateChild("PART_Rect") is Rectangle r) r.MouseDown += Rect_MouseDown;
        }

        /// <summary> Текст всплывающей подсказки </summary>
        public string BubbleText
        {
            get => (string)GetValue(BubbleTextProperty);
            set => SetValue(BubbleTextProperty, value);
        }

        public static readonly DependencyProperty BubbleTextProperty =
            DependencyProperty.Register("BubbleText", typeof(string), typeof(WPRDialogPanel), new PropertyMetadata(""));


        /// <summary> Оставаться ли открытым при клике на заблокированную область </summary>
        private bool StaysOpen { get; set; }


        private void Rect_MouseDown(object sender, MouseEventArgs e)
        {
            if (StaysOpen)
            {
                if (Template.Resources["ShakeAnim"] is Storyboard s && GetTemplateChild("Grid") is Grid g)
                {
                    s.Begin(g);
                }
            }
            else
            {
                Hide();
            }
        }

        /// <summary>
        /// Статический метод для показа контента в любом окне
        /// </summary>
        /// <param name="window">Окно, в котором нужно показать контент</param>
        /// <param name="Content">Объект - содержимое для показа в качестве диалога</param>
        /// <param name="staysOpen">Не позволять закрыть содержимое при клике за его пределы</param>
        public static void ShowOnWindow(Window window, object Content, bool staysOpen)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));
            if (window.Template.FindName("WindowDialogPanel", window) is WPRDialogPanel panel)
            {
                panel.Show(Content, staysOpen);
            }
        }

        #region Диалоговое окно
        /// <summary> Показан ли какой-либо диалог </summary>
        public bool IsShowing
        {
            get => (bool)GetValue(IsShowingProperty);
            set => SetValue(IsShowingProperty, value);
        }
        public static readonly DependencyProperty IsShowingProperty =
            DependencyProperty.Register("IsShowing", typeof(bool), typeof(WPRDialogPanel), new PropertyMetadata(false));

        /// <summary>
        /// Показать контент и затемнить родителя
        /// </summary>
        /// <param name="staysOpen">Не позволять закрыть содержимое при клике за его пределы</param>
        public void Show(bool staysOpen)
        {
            StaysOpen = staysOpen;
            IsShowing = true;
        }

        /// <summary>
        /// Показать контент и затемнить родителя
        /// </summary>
        /// <param name="content">Объект - содержимое для показа в качестве диалога</param>
        /// <param name="staysOpen">Не позволять закрыть содержимое при клике за его пределы</param>
        public void Show(object content, bool staysOpen)
        {
            Header = content;
            Show(staysOpen);
        }


        /// <summary>
        /// Скрыть контент и осветлить родителя
        /// </summary>
        public void Hide()
        {
            Focus();
            IsShowing = false;
            StaysOpen = false;
        }
        #endregion

        #region Всплывающая подсказка

        /// <summary>
        /// Показать всплывающую подсказку
        /// </summary>
        /// <param name="Text">Текст сообщения</param>
        /// <param name="Duration">Длительность</param>
        /// <param name="ButtonCommandText">Текст кнопки команды</param>
        public void ShowBubble(string Text, int Duration = 2000, string ButtonCommandText = "")
        {
            _StackBubblesQueue.Enqueue(new StackBubbles() { Text = Text, Duration = Duration, Buttontext = ButtonCommandText });
            if (_StackBubblesQueue.Count == 1) ShowBubbleinStack();
        }

        private void HideBubble()
        {
            Border clip = GetTemplateChild("PART_Bubble") as Border;
            _Animout.Completed -= Animout_Completed;
            _Animout = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
            _Animout.Completed += Animout_Completed;
            clip?.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _Animout);
        }

        private void ShowBubbleinStack()
        {
            if (_StackBubblesQueue.Count == 0) return;
            StackBubbles stack = _StackBubblesQueue.Peek();
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


            if (GetTemplateChild("PART_Bubble") is Border clip)
            {
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

        }

        private void BubbleButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(sender, e);
            ButtonClicked = null;
            HideBubble();
        }

        private void Animout_Completed(object sender, EventArgs e)
        {
            _StackBubblesQueue.Dequeue();
            ShowBubbleinStack();
        }

        private DoubleAnimation _Animout;

        private readonly Queue<StackBubbles> _StackBubblesQueue = new(); // Очередь подсказок
        private struct StackBubbles
        {
            public string Text;
            public int Duration;
            public string Buttontext;
        }
        #endregion
    }
}
