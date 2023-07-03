using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPR.ColorTheme;
using WPR.Domain.Interfaces;
using WPR.Domain.Models.Themes;

namespace WPR.Controls;

/// <summary> Контрол для обёртки диалоговых окон </summary>
internal class WPRDialogPanel : HeaderedContentControl
{
    public enum Status
    {
        NotShowing,
        Showing,
        Hiding
    }

    private IWPRDialog _WPRDialog;
    private bool _StaysOpen;
    private ContentPresenter _Header;
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
        _Header = GetTemplateChild("PART_HeaderContent") as ContentPresenter;

        if (_HeaderPopup == null)
            throw new ArgumentNullException(nameof(_HeaderPopup), "Попап не найден в шаблоне!");

        _HeaderPopup.PopupClosed += HeaderPopupOnClosed;
    }


    #region Диалоговое окно

    private readonly Stack<(object content, bool staysOpen)> _DialogContentStack = new(); // Очередь объектов для отображения


    #region CurrentStatus : Status - Статус показа контента

    /// <summary>Статус показа контента</summary>
    public static readonly DependencyProperty CurrentStatusProperty =
        DependencyProperty.Register(
            nameof(CurrentStatus),
            typeof(Status),
            typeof(WPRDialogPanel),
            new PropertyMetadata(default(Status)));

    /// <summary>Статус показа контента</summary>
    public Status CurrentStatus
    {
        get => (Status)GetValue(CurrentStatusProperty);
        set => SetValue(CurrentStatusProperty, value);
    }

    #endregion


    /// <summary>
    /// Показать контент и затемнить родителя
    /// </summary>
    /// <param name="content">Объект - содержимое для показа в качестве диалога</param>
    /// <param name="staysOpen">Не позволять закрыть содержимое при клике за его пределы</param>
    public void Show(object content, bool staysOpen)
    {

        switch (CurrentStatus)
        {
            case Status.NotShowing:
                _DialogContentStack.Push((content, staysOpen));
                ShowFromQueue();
                break;

            case Status.Showing:
                // Поместить текущий диалог в очередь и запустить новый
                _DialogContentStack.Push((Header, _StaysOpen));
                _DialogContentStack.Push((content, staysOpen));
                Hide();

                break;
            case Status.Hiding:
                _DialogContentStack.Push((content, staysOpen));

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Скрыть контент и осветлить родителя
    /// </summary>
    public void Hide()
    {
        if (CurrentStatus != Status.Showing) return;
        CurrentStatus = Status.Hiding;
        _HeaderPopup.Hide();
        _WPRDialog = null;
    }

    // Показать следующий контент
    private void ShowFromQueue()
    {
        if (!_DialogContentStack.TryPop(out var nextContent))
        {
            Header = null;
            Focus();
            return;
        }
        _HeaderPopup.Show();
        _StaysOpen = nextContent.staysOpen;
        CurrentStatus = Status.Showing;

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
        _Header.Focus();
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
    /// <param name="BubbleBackground">Заливка сообщения</param>
    public void ShowBubble(string Text, int Duration = 2000, string ButtonCommandText = "", Action<bool> Callback = null, StyleBrushes BubbleBackground = StyleBrushes.BackgroundContrastColorBrush)
    {
        _StackBubblesQueue.Enqueue(new StackBubbles
        {
            Text = Text, 
            Duration = Duration,
            Buttontext = ButtonCommandText,
            Action = Callback,
            Background = BubbleBackground
        });
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

        if (GetTemplateChild("PART_Bubble") is not Border clip) return;
        clip.Background = StyleHelper.GetBrushFromResource(stack.Background);


        if (GetTemplateChild("BubbleButton") is Button commandbutton)
        {

            if (!string.IsNullOrEmpty(stack.Buttontext))
            {
                commandbutton.Content = stack.Buttontext;
                commandbutton.Visibility = Visibility.Visible;
            }
            else
                commandbutton.Visibility = Visibility.Collapsed;
        }

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
        if(_StackBubblesQueue.Count == 0)
            return;

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
        public StyleBrushes Background;
    }
    #endregion


    #region PrivateMethods

    private void HeaderPopupOnClosed()
    {
        CurrentStatus = Status.NotShowing;
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
            RaiseEvent(_WPRDialog, "Completed");
            Hide();
        }
    }

    private const BindingFlags StaticFlags = BindingFlags.Instance | BindingFlags.NonPublic;

    private static void RaiseEvent(object instance, string eventName)
    {
        var type = instance?.GetType();
        if (type == null) return;
        var eventField = type.GetField(eventName, StaticFlags);
        if (eventField == null)
            throw new Exception($"Event with name {eventName} could not be found.");
        if (eventField.GetValue(instance) is not Action<bool> multicastDelegate)
            return;

        var invocationList = multicastDelegate.GetInvocationList();

        foreach (var invocationMethod in invocationList)
            invocationMethod.DynamicInvoke(false);
    }

    #endregion
}