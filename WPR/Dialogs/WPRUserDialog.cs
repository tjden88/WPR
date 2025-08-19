using Microsoft.Win32;
using System.Windows;
using WPR.Theme;

namespace WPR.Dialogs;

/// <summary> Реализация сервис диалогов, которая работает только в стилях WPR </summary>
internal class WPRUserDialog : IUserDialog
{
    /// <summary>
    /// Возвращает текущее активное окно, гарантируя доступ из UI-потока
    /// </summary>
    public static Window ActiveWindow
    {
        get
        {
            if (Application.Current == null)
                return null;

            if (Application.Current.Dispatcher.CheckAccess())
            {
                var active = Application.Current.Windows
                    .Cast<Window>()
                    .FirstOrDefault(w => w.IsActive);
                return active ?? Application.Current.MainWindow ?? Application.Current.Windows.Cast<Window>().FirstOrDefault();
            }

            return Application.Current.Dispatcher.Invoke(() =>
            {
                var active = Application.Current.Windows
                    .Cast<Window>()
                    .FirstOrDefault(w => w.IsActive);
                return active ?? Application.Current.MainWindow ?? Application.Current.Windows.Cast<Window>().FirstOrDefault();
            });
        }
    }

    private DependencyObject _AssociatedElement;
    public DependencyObject AssociatedElement
    {
        get => _AssociatedElement ?? ActiveWindow;
        set => _AssociatedElement = value;
    }

    /// <summary> Будет показывать модальные окна </summary>
    public bool IsModal { get; set; }


    private Task<bool> Show(IWPRDialog dialog, CancellationToken cancellationToken) =>
        IsModal 
            ? DialogHelper.ShowModal(AssociatedElement, dialog, cancellationToken)
            : DialogHelper.Show(AssociatedElement, dialog, cancellationToken);

    public async Task InformationAsync(string message, string Title = null, CancellationToken cancellationToken = default)
    {
        var dlg = CreateDialog(() => new MessageDialog
        {
            Title = Title,
            Content = message,
            DialogType = DialogType.Information
        });

        await Show(dlg, cancellationToken);
    }

    public async Task<bool> QuestionAsync(string message, string Title = null, CancellationToken cancellationToken = default)
    {
        var dlg = CreateDialog(() => new MessageDialog
        {
            Title = Title,
            Content = message,
            DialogType = DialogType.Question
        });

        return await Show(dlg, cancellationToken);
    }

    public async Task<bool?> QuestionAsync(string message, DialogType dialogType, string Title = null, CancellationToken cancellationToken = default)
    {
        var dlg = CreateDialog(() => new MessageDialog
        {
            Title = Title,
            Content = message,
            DialogType = dialogType
        });

        var result = await Show(dlg, cancellationToken);
        if (dlg.IsCancelled) return null;
        return result;
    }

    public async Task<bool?> CustomQuestionAsync(string message, string Title, string AcceptCaption, string RejectCaption = null,
        string CancelCaption = null, CancellationToken cancellationToken = default)
    {
        var dialogType = (RejectCaption, CancelCaption) switch
        {
            (null, null) => DialogType.Information,
            (not null, null) => DialogType.Question,
            (null, not null) => DialogType.InformationCancel,
            (not null, not null) => DialogType.QuestionCancel
        };

        var dlg = CreateDialog(() => new MessageDialog
        {
            Title = Title,
            Content = message,
            AcceptButtonText = AcceptCaption,
            CancelButtonText = CancelCaption,
            QuestionAcceptButtonText = AcceptCaption,
            QuestionCancelButtonText = RejectCaption,
            DialogType = dialogType
        });

        var result = await Show(dlg, cancellationToken);
        if (dlg.IsCancelled) return null;
        return result;
    }

    public async Task ErrorMessageAsync(string message, string Title = "Ошибка", CancellationToken cancellationToken = default)
    {
        var dlg = CreateDialog(() => new MessageDialog
        {
            Title = Title,
            Content = message,
            DialogType = DialogType.Information,
            IsErrorMessage = true
        });

        await Show(dlg, cancellationToken);
    }

    public async Task<bool> CustomDialogAsync(IWPRDialog Dialog, CancellationToken cancellationToken = default) => 
         await Show(Dialog, cancellationToken);

    public async Task<string> InputTextAsync(string message, string defaultValue = null, string title = null, bool MultiLine = false,
        CancellationToken cancellationToken = default)
    {
        var input = CreateDialog(() =>  new InputDialog()
        {
            Title = title,
            Content = message,
            MultiLine = MultiLine,
            Text = defaultValue
        });

        var result = await Show(input, cancellationToken);

        return result ? input.Text : null;
    }

    public async Task<string> InputValidatedTextAsync(Action<InputDialogFilterOptions> options, CancellationToken cancellationToken = default)
    {
        var filter = new InputDialogFilterOptions();
        options?.Invoke(filter);

        var input = CreateDialog(() => new InputDialog(filter.Validation)
        {
            Title = filter.Title,
            Content = filter.Message,
            MultiLine = filter.MultiLine,
            Text = filter.DefaultValue
        });

        var result = await Show(input, cancellationToken);

        return result ? input.Text : null;
    }

    public async Task ShowNotificationAsync(string message, StyleBrushes background = StyleBrushes.BackgroundContrastColorBrush,
        int delay = 2000) =>
        await DialogHelper.ShowNotification(AssociatedElement, message, background, delay);

    public async Task<bool> ShowQuestionNotificationAsync(string message, string AcceptCaption,
        StyleBrushes background = StyleBrushes.BackgroundContrastColorBrush, int delay = 3000) =>
        await DialogHelper.ShowNotification(AssociatedElement, message, background, delay, AcceptCaption);

    public Task<string> ShowOpenFileDialogAsync(string Title, IEnumerable<FileFilter> Filters = null, string InitFileName = "")
    {
        var ofd = CreateDialog(() =>  new OpenFileDialog
        {
            FileName = InitFileName,
            Title = Title
        });

        if (Filters != null)
        {
            var ofdFilter = Filters
                .Select(f => $"{f.Description}|{string.Concat(f.FileMathPatterns.Select(e => $"{e};"))}");
            ofd.Filter = string.Join("|", ofdFilter);
        }

        var dialogResult = ofd.ShowDialog(ActiveWindow);

        return Task.FromResult(dialogResult == true ? ofd.FileName : null);
    }

    public Task<string> ShowSaveFileDialogAsync(string Title, IEnumerable<FileFilter> Filters = null, string InitFileName = "")
    {
        var sfd = CreateDialog(() =>  new SaveFileDialog
        {
            FileName = InitFileName,
            Title = Title
        });

        if (Filters != null)
        {
            var ofdFilter = Filters
                .Select(f => $"{f.Description}|{string.Concat(f.FileMathPatterns.Select(e => $"{e};"))}");
            sfd.Filter = string.Join("|", ofdFilter);
        }


        var dialogResult = sfd.ShowDialog(ActiveWindow);

        return Task.FromResult(dialogResult == true ? sfd.FileName : null);
    }

    public Task<string> ShowFolderSelectDialogAsync(string Title, string InitPathName = "")
    {
        var dialog = CreateDialog(() =>  new OpenFolderDialog
        {
            DefaultDirectory = InitPathName,
            Title = Title

        });

        return !dialog.ShowDialog(ActiveWindow) == true 
            ? Task.FromResult<string>(null) 
            : Task.FromResult(dialog.FolderName)!;
    }


    private T CreateDialog<T>(Func<T> action)
    {
        var dispatcher = Application.Current.Dispatcher;
        if (dispatcher == null || dispatcher.HasShutdownStarted)
            throw new InvalidOperationException("Application is shutting down or not initialized.");

        return dispatcher.CheckAccess() ? action.Invoke() : dispatcher.Invoke(action);
    }
}