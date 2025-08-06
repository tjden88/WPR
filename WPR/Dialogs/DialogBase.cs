using System.Windows.Controls;
using System.Windows.Input;
using WPR.Infrastructure.Commands;

namespace WPR.Dialogs;

public abstract class DialogBase : Control, IWPRDialog
{
    public event Action<bool> Completed;

    protected DialogBase()
    {
        SubmitCommand = new BaseCommand(() => Completed?.Invoke(true));
        CancelCommand = new BaseCommand(() => Completed?.Invoke(false));
    }

    /// <summary> Заголовок диалога </summary>
    public string Title { get; init; }

    /// <summary> Контент диалога </summary>
    public string Content { get; init; }

    public ICommand SubmitCommand { get; }

    public ICommand CancelCommand { get; }

    protected void RaiseCompleted(bool result) => Completed?.Invoke(result);
}