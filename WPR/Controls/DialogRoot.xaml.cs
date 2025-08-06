using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPR.Dialogs;
using WPR.Theme;

namespace WPR;

/// <summary> Контрол для обёртки диалоговых окон </summary>
public class DialogRoot : HeaderedContentControl
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

    static DialogRoot()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogRoot), new FrameworkPropertyMetadata(typeof(DialogRoot)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (GetTemplateChild("NotificationButton") is Button b) b.Click += NotificationButton_Click;
        if (GetTemplateChild("PART_Notification") is Border br) br.MouseUp += (_, _) => HideNotification();
        if (GetTemplateChild("PART_Rect") is Rectangle r) r.MouseDown += Rect_MouseDown;

        _HeaderPopup = GetTemplateChild("PART_Popup") as WPRPopup;
        _Header = GetTemplateChild("PART_HeaderContent") as ContentPresenter;

        if (_HeaderPopup == null)
            throw new ArgumentNullException(nameof(_HeaderPopup), "Попап не найден в шаблоне!");

        _HeaderPopup.PopupClosed += HeaderPopupOnClosed;
    }


    #region DialogSource : IUserDialog - Сообщения из этого источника будут появляться в этой панели

    /// <summary>Сообщения из этого источника будут появляться в этой панели</summary>
    public static readonly DependencyProperty DialogSourceProperty =
        DependencyProperty.Register(
            nameof(DialogSource),
            typeof(IUserDialog),
            typeof(DialogRoot),
            new PropertyMetadata(null, OnDialogSourceChanged));

    private static void OnDialogSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is WPRUserDialog dlg && d is DialogRoot panel)
            dlg.AssociatedElement = panel;
    }

    /// <summary>Сообщения из этого источника будут появляться в этой панели</summary>
    [Category("WPRDialogPanel")]
    [Description("Сообщения из этого источника будут появляться в этой панели")]
    public IUserDialog DialogSource
    {
        get => (IUserDialog) GetValue(DialogSourceProperty);
        set => SetValue(DialogSourceProperty, value);
    }

    #endregion


    #region Диалоговое окно

    private readonly Stack<(object content, bool staysOpen)> _DialogContentStack = new(); // Очередь объектов для отображения


    #region CurrentStatus : Status - Статус показа контента

    /// <summary>Статус показа контента</summary>
    internal static readonly DependencyProperty CurrentStatusProperty =
        DependencyProperty.Register(
            nameof(CurrentStatus),
            typeof(Status),
            typeof(DialogRoot),
            new PropertyMetadata(default(Status)));

    /// <summary>Статус показа контента</summary>
    internal Status CurrentStatus
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

    #region NotificationText
    /// <summary> Текст всплывающей подсказки </summary>
    internal string NotificationText
    {
        get => (string)GetValue(NotificationTextProperty);
        set => SetValue(NotificationTextProperty, value);
    }

    internal static readonly DependencyProperty NotificationTextProperty =
        DependencyProperty.Register(nameof(NotificationText), typeof(string), typeof(DialogRoot), new PropertyMetadata(""));


    #endregion


    /// <summary>
    /// Показать всплывающую подсказку
    /// </summary>
    /// <param name="Text">Текст сообщения</param>
    /// <param name="Duration">Длительность</param>
    /// <param name="ButtonCommandText">Текст кнопки команды</param>
    /// <param name="Callback">True, если кнопка была нажата</param>
    /// <param name="NotificationBackground">Заливка сообщения</param>
    public void ShowNotification(string Text, int Duration = 2000, string ButtonCommandText = "", Action<bool> Callback = null, StyleBrushes NotificationBackground = StyleBrushes.BackgroundContrastColorBrush)
    {
        _StackNotificationsQueue.Enqueue(new StackNotifications
        {
            Text = Text, 
            Duration = Duration,
            Buttontext = ButtonCommandText,
            Action = Callback,
            Background = NotificationBackground
        });
        if (_StackNotificationsQueue.Count == 1) ShowNotificationInStack();
    }

    private void HideNotification()
    {
        var clip = GetTemplateChild("PART_Notification") as Border;
        _Animout.Completed -= Animout_Completed;
        _Animout = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1));
        _Animout.Completed += Animout_Completed;
        clip?.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _Animout);
    }

    private void ShowNotificationInStack()
    {
        if (!_StackNotificationsQueue.TryPeek(out var stack)) return;
        NotificationText = stack.Text;

        if (GetTemplateChild("PART_Notification") is not Border clip) return;
        clip.Background = StyleHelper.GetBrushFromResource(stack.Background);


        if (GetTemplateChild("NotificationButton") is Button button)
        {

            if (!string.IsNullOrEmpty(stack.Buttontext))
            {
                button.Content = stack.Buttontext;
                button.Visibility = Visibility.Visible;
            }
            else
                button.Visibility = Visibility.Collapsed;
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

    private void NotificationButton_Click(object sender, RoutedEventArgs e)
    {
        _StackNotificationsQueue.Peek().Action?.Invoke(true);
        HideNotification();
    }

    private void Animout_Completed(object sender, EventArgs e)
    {
        if(_StackNotificationsQueue.Count == 0)
            return;

        _StackNotificationsQueue.Dequeue().Action?.Invoke(false);
        ShowNotificationInStack();
    }

    private DoubleAnimation _Animout;

    private readonly Queue<StackNotifications> _StackNotificationsQueue = new(); // Очередь подсказок
    private struct StackNotifications
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