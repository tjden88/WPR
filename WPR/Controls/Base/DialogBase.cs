using System.Windows.Controls;
using System.Windows.Input;
using WPR.Dialogs;
using WPR.Infrastructure.Commands;

namespace WPR.Controls.Base;

public abstract class DialogBase : Control, IWPRDialog
{
    public event Action<bool> Completed;

    protected DialogBase()
    {
        SubmitCommand = new BaseCommand(() => Completed?.Invoke(true), CanSubmit);
        CancelCommand = new BaseCommand(() => Completed?.Invoke(false));
    }

    /// <summary> Заголовок диалога </summary>
    public string Title { get; init; }

    /// <summary> Контент диалога </summary>
    public string Content { get; init; }

    public ICommand SubmitCommand { get; }

    public ICommand CancelCommand { get; }

    protected virtual bool CanSubmit() => true;

    protected void RaiseCompleted(bool result) => Completed?.Invoke(result);
}