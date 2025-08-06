using Microsoft.Win32;
using System;
using System.Windows;
using WPR.Theme;

namespace WPR.Dialogs;

/// <summary> Реализация сервис диалогов, которая работает только в стилях WPR </summary>
internal class WPRUserDialog : IUserDialog
{
    private static Window ActiveWindow => Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);

    private DependencyObject _AssociatedElement;
    public DependencyObject AssociatedElement
    {
        get => _AssociatedElement ?? ActiveWindow;
        set => _AssociatedElement = value;
    }

    public bool IsModal { get; set; }


    //public async Task InformationAsync(string message, string? Title = null) => 
    //    await DoDispatcheredAction(WPRDialogHelper.InformationAsync(Active, message, Title));


    //public async Task<bool> QuestionAsync(string message, string? Title = null) =>
    //    await GetDispatcheredResult(() => WPRDialogHelper.QuestionAsync(Active, message, Title));


    //public async Task<bool?> QuestionAsync(string message, IUserDialog.DialogTypes DilaogType, string? Title = null) =>
    //    DilaogType switch
    //    {
    //        IUserDialog.DialogTypes.YesNo => await GetDispatcheredResult(() => WPRDialogHelper.QuestionAsync(Active, message, Title)),
    //        IUserDialog.DialogTypes.YesNoCancel => await GetDispatcheredResult(() => WPRDialogHelper.QuestionCancelAsync(Active, message, Title)),
    //        IUserDialog.DialogTypes.OkCancel => await GetDispatcheredResult(() => WPRDialogHelper.InformationCancelAsync(Active, message, Title)),
    //        _ => throw new ArgumentOutOfRangeException(nameof(DilaogType), DilaogType, "Неверный тип диалога")
    //    };


    //public async Task<bool?> CustomQuestionAsync(string message, string? Title, string TrueCaption, string? FalseCaption = null,
    //    string? NullCaption = null) =>
    //    await GetDispatcheredResult(() => WPRDialogHelper
    //        .ShowCustomButtonsDialog(Active, message, Title, TrueCaption, FalseCaption, NullCaption));


    //public async Task ErrorMessageAsync(string message, string? Title = "Ошибка") =>
    //    await DoDispatcheredAction(WPRDialogHelper.ErrorAsync(Active, message, Title));


    //public async Task<bool> CustomDialogAsync(IWPRDialog Dialog)
    //    => await GetDispatcheredResult(() => WPRDialogHelper.ShowCustomDialogAsync(Active, Dialog));


    //public Task<string?> InputValidatedTextAsync(Action<InputDialogFilterOptions> options)
    //{
    //    var filterOptions = new InputDialogFilterOptions();
    //    options.Invoke(filterOptions);
    //    return InputValidatedTextAsync(filterOptions);
    //}


    //public async Task<string?> InputTextAsync(string title, string? DefaultValue = null, string? message = null, bool MultiLine = false) =>
    //    await GetDispatcheredResult(() => WPRDialogHelper.InputTextAsync(Active, title, message, DefaultValue, MultiLine));

    //private async Task<string?> InputValidatedTextAsync(InputDialogFilterOptions DialogFilter) =>
    //    await GetDispatcheredResult(() => WPRDialogHelper.InputTextAsync(Active,
    //        DialogFilter.Title,
    //        DialogFilter.Message,
    //        DialogFilter.DefaultValue,
    //        DialogFilter.Validation.Select(f => new PredicateValidationRule<string>(f.Rule, f.ErrorMessage)),
    //        DialogFilter.MultiLine));

    //public Task ShowNotificationAsync(string message, int delay = 2000, StyleBrushes Backgound = StyleBrushes.BackgroundContrastColorBrush)
    //{
    //    Application.Current.Dispatcher.Invoke(() => WPRDialogHelper.Bubble(Active, message, delay, Backgound));
    //    return Task.CompletedTask;
    //}

    //public async Task<bool> ShowQuestionNotificationAsync(string message, string AcceptCaption, int delay = 3000, StyleBrushes Backgound = StyleBrushes.BackgroundContrastColorBrush)
    //{
    //    return await GetDispatcheredResult(() =>
    //    {
    //        var result = new TaskCompletionSource<bool>();
    //        WPRDialogHelper.Bubble(Active, message, AcceptCaption, b => result.TrySetResult(b), delay, Backgound);
    //        return result.Task;
    //    });
    //}

    //public Task<string?> ShowOpenFileDialogAsync(string Title, IEnumerable<FileFilter>? Filters = null, string InitFileName = "")
    //{
    //    var ofd = new OpenFileDialog()
    //    {
    //        FileName = InitFileName,
    //        Title = Title
    //    };

    //    if (Filters != null)
    //    {
    //        var ofdFilter = Filters
    //            .Select(f => $"{f.Description}|{string.Concat(f.FileMathPatterns.Select(e => $"{e};"))}");
    //        ofd.Filter = string.Join("|", ofdFilter);
    //    }

    //    Window? window = null;
    //    Application.Current.Dispatcher.Invoke(() => window = Active);

    //    var dialogResult = ofd.ShowDialog(window);

    //    return Task.FromResult(dialogResult == true ? ofd.FileName : null);
    //}

    //public Task<string?> ShowSaveFileDialogAsync(string Title, IEnumerable<FileFilter>? Filters = null, string InitFileName = "")
    //{
    //    var sfd = new SaveFileDialog
    //    {
    //        FileName = InitFileName,
    //        Title = Title
    //    };

    //    if (Filters != null)
    //    {
    //        var ofdFilter = Filters
    //            .Select(f => $"{f.Description}|{string.Concat(f.FileMathPatterns.Select(e => $"{e};"))}");
    //        sfd.Filter = string.Join("|", ofdFilter);
    //    }

    //    Window? window = null;
    //    Application.Current.Dispatcher.Invoke(() => window = Active);

    //    var dialogResult = sfd.ShowDialog(window);

    //    return Task.FromResult(dialogResult == true ? sfd.FileName : null);
    //}

    //public Task<string?> ShowFolderSelectDialogAsync(string Title, string InitPathName = "")
    //{
    //    var dialog = new OpenFolderDialog()
    //    {
    //        DefaultDirectory = InitPathName,
    //        Title = Title

    //    };

    //    if (!dialog.ShowDialog(Active) == true)
    //        return Task.FromResult<string?>(null);

    //    return Task.FromResult(dialog.FolderName)!;
    //}


    //private static async Task DoDispatcheredAction(Task action) => await Application.Current.Dispatcher.Invoke(async () => await action, DispatcherPriority.Normal);


    //private static async Task<T?> GetDispatcheredResult<T>(Func<Task<T>> action)
    //{
    //    T? result = default;

    //    await Application.Current.Dispatcher.Invoke(async () => result = await action.Invoke(), DispatcherPriority.Normal);

    //    return result;
    //}

    private Task<bool> Show(IWPRDialog dialog, CancellationToken cancellationToken) =>
        IsModal 
            ? UserDialog.ShowModal(AssociatedElement, dialog, cancellationToken)
            : UserDialog.Show(AssociatedElement, dialog, cancellationToken);

    public async Task InformationAsync(string message, string Title = null, CancellationToken cancellationToken = default)
    {
        var dlg = new MessageDialog
        {
            Title = Title,
            Content = message,
            DialogType = DialogType.Information
        };

        await Show(dlg, cancellationToken);
    }

    public async Task<bool> QuestionAsync(string message, string Title = null, CancellationToken cancellationToken = default)
    {
        var dlg = new MessageDialog
        {
            Title = Title,
            Content = message,
            DialogType = DialogType.Question
        };

        return await Show(dlg, cancellationToken);
    }

    public async Task<bool?> QuestionAsync(string message, DialogType dialogType, string Title = null, CancellationToken cancellationToken = default)
    {
        var dlg = new MessageDialog
        {
            Title = Title,
            Content = message,
            DialogType = dialogType
        };

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

        var dlg = new MessageDialog
        {
            Title = Title,
            Content = message,
            AcceptButtonText = AcceptCaption,
            CancelButtonText = RejectCaption,
            QuestionAcceptButtonText = AcceptCaption,
            QuestionCancelButtonText = RejectCaption,
            DialogType = dialogType
        };

        var result = await Show(dlg, cancellationToken);
        if (dlg.IsCancelled) return null;
        return result;
    }

    public async Task ErrorMessageAsync(string message, string Title = "Ошибка", CancellationToken cancellationToken = default)
    {
        var dlg = new MessageDialog
        {
            Title = Title,
            Content = message,
            DialogType = DialogType.Information,
            IsErrorMessage = true
        };

        await Show(dlg, cancellationToken);
    }

    public async Task<bool> CustomDialogAsync(IWPRDialog Dialog, CancellationToken cancellationToken = default) => 
         await Show(dlg, cancellationToken);

    public async Task<string> InputTextAsync(string title, string DefaultValue = null, string message = null, bool MultiLine = false,
        CancellationToken cancellationToken = default)
    {
        var input = new InputDialog()
        {
            Title = title,
            Content = message,
            MultiLine = MultiLine,
            Text = DefaultValue
        };

        var result = await Show(input, cancellationToken);

        return result ? input.Text : null;
    }

    public async Task<string> InputValidatedTextAsync(Action<InputDialogFilterOptions> options, CancellationToken cancellationToken = default)
    {
        var filter = new InputDialogFilterOptions();
        options?.Invoke(filter);

        var input = new InputDialog(filter.Validation)
        {
            Title = filter.Title,
            Content = filter.Message,
            MultiLine = filter.MultiLine,
            Text = filter.DefaultValue
        };

        var result = await Show(input, cancellationToken);

        return result ? input.Text : null;
    }

    public async Task ShowNotificationAsync(string message, StyleBrushes background = StyleBrushes.BackgroundContrastColorBrush,
        int delay = 2000)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ShowQuestionNotificationAsync(string message, string AcceptCaption,
        StyleBrushes background = StyleBrushes.BackgroundContrastColorBrush, int delay = 3000)
    {
        throw new NotImplementedException();
    }

    public Task<string> ShowOpenFileDialogAsync(string Title, IEnumerable<FileFilter> Filters = null, string InitFileName = "")
    {
        var ofd = new OpenFileDialog()
        {
            FileName = InitFileName,
            Title = Title
        };

        if (Filters != null)
        {
            var ofdFilter = Filters
                .Select(f => $"{f.Description}|{string.Concat(f.FileMathPatterns.Select(e => $"{e};"))}");
            ofd.Filter = string.Join("|", ofdFilter);
        }

        var dialogResult = ofd.ShowDialog(ActiveWindow);

        return Task.FromResult(dialogResult == true ? ofd.FileName : null);
    }

    public Task<string> ShowSaveFileDialogAsync(string Title, IEnumerable<FileFilter>? Filters = null, string InitFileName = "")
    {
        var sfd = new SaveFileDialog
        {
            FileName = InitFileName,
            Title = Title
        };

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
        var dialog = new OpenFolderDialog()
        {
            DefaultDirectory = InitPathName,
            Title = Title

        };

        if (!dialog.ShowDialog(ActiveWindow) == true)
            return Task.FromResult<string?>(null);

        return Task.FromResult(dialog.FolderName)!;
    }
}