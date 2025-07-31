using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using WPR.Abstractions.Interfaces;
using WPR.Abstractions.Models.Dialogs;
using WPR.Abstractions.Models.Files;
using WPR.Abstractions.Models.Themes;
using WPR.Dialogs;
using WPR.Validation;

namespace WPR.Services.Implementations;

public class UserDialog : IUserDialog
{

    private static Window? Active => Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);


    public async Task InformationAsync(string message, string? Title = null) => 
        await DoDispatcheredAction(WPRDialogHelper.InformationAsync(Active, message, Title));


    public async Task<bool> QuestionAsync(string message, string? Title = null) =>
        await GetDispatcheredResult(() => WPRDialogHelper.QuestionAsync(Active, message, Title));


    public async Task<bool?> QuestionAsync(string message, IUserDialog.DialogTypes DilaogType, string? Title = null) =>
        DilaogType switch
        {
            IUserDialog.DialogTypes.YesNo => await GetDispatcheredResult(() => WPRDialogHelper.QuestionAsync(Active, message, Title)),
            IUserDialog.DialogTypes.YesNoCancel => await GetDispatcheredResult(() => WPRDialogHelper.QuestionCancelAsync(Active, message, Title)),
            IUserDialog.DialogTypes.OkCancel => await GetDispatcheredResult(() => WPRDialogHelper.InformationCancelAsync(Active, message, Title)),
            _ => throw new ArgumentOutOfRangeException(nameof(DilaogType), DilaogType, "Неверный тип диалога")
        };



    public async Task<bool?> CustomQuestionAsync(string message, string? Title, string TrueCaption, string? FalseCaption = null,
        string? NullCaption = null) =>
        await GetDispatcheredResult(() => WPRDialogHelper
            .ShowCustomButtonsDialog(Active, message, Title, TrueCaption, FalseCaption, NullCaption));



    public async Task ErrorMessageAsync(string message, string? Title = "Ошибка") =>
        await DoDispatcheredAction(WPRDialogHelper.ErrorAsync(Active, message, Title));


    public async Task<bool> CustomDialogAsync(IWPRDialog Dialog)
        => await GetDispatcheredResult(() => WPRDialogHelper.ShowCustomDialogAsync(Active, Dialog));


    public Task<string?> InputValidatedTextAsync(Action<InputDialogFilterOptions> options)
    {
        var filterOptions = new InputDialogFilterOptions();
        options.Invoke(filterOptions);
        return InputValidatedTextAsync(filterOptions);
    }


    public async Task<string?> InputTextAsync(string title, string? DefaultValue = null, string? message = null, bool MultiLine = false) =>
        await GetDispatcheredResult(() => WPRDialogHelper.InputTextAsync(Active, title, message, DefaultValue, MultiLine));

    private async Task<string?> InputValidatedTextAsync(InputDialogFilterOptions DialogFilter) =>
        await GetDispatcheredResult(() => WPRDialogHelper.InputTextAsync(Active,
            DialogFilter.Title,
            DialogFilter.Message,
            DialogFilter.DefaultValue,
            DialogFilter.ValidationRules.Select(f => new PredicateValidationRule<string>(f.Rule, f.ErrorMessage)),
            DialogFilter.MultiLine));

    public Task ShowNotificationAsync(string message, int delay = 2000, StyleBrushes Backgound = StyleBrushes.BackgroundContrastColorBrush)
    {
        Application.Current.Dispatcher.Invoke(() => WPRDialogHelper.Bubble(Active, message, delay, Backgound));
        return Task.CompletedTask;
    }

    public async Task<bool> ShowQuestionNotificationAsync(string message, string AcceptCaption, int delay = 3000, StyleBrushes Backgound = StyleBrushes.BackgroundContrastColorBrush)
    {
        return await GetDispatcheredResult(() =>
        {
            var result = new TaskCompletionSource<bool>();
            WPRDialogHelper.Bubble(Active, message, AcceptCaption, b => result.TrySetResult(b), delay, Backgound);
            return result.Task;
        });
    }

    public Task<string?> ShowOpenFileDialogAsync(string Title, IEnumerable<FileFilter>? Filters = null, string InitFileName = "")
    {
        var ofd = new OpenFileDialog()
        {
            FileName = InitFileName,
            Title = Title
        };

        if (Filters != null)
        {
            var ofdFilter = Filters
                .Select(f => $"{f.Description}|{string.Concat(f.FileMathPattrerns.Select(e => $"{e};"))}");
            ofd.Filter = string.Join("|", ofdFilter);
        }

        Window? window = null;
        Application.Current.Dispatcher.Invoke(() => window = Active);

        var dialogResult = ofd.ShowDialog(window);

        return Task.FromResult(dialogResult == true ? ofd.FileName : null);
    }

    public Task<string?> ShowSaveFileDialogAsync(string Title, IEnumerable<FileFilter>? Filters = null, string InitFileName = "")
    {
        var sfd = new SaveFileDialog
        {
            FileName = InitFileName,
            Title = Title
        };

        if (Filters != null)
        {
            var ofdFilter = Filters
                .Select(f => $"{f.Description}|{string.Concat(f.FileMathPattrerns.Select(e => $"{e};"))}");
            sfd.Filter = string.Join("|", ofdFilter);
        }

        Window? window = null;
        Application.Current.Dispatcher.Invoke(() => window = Active);

        var dialogResult = sfd.ShowDialog(window);

        return Task.FromResult(dialogResult == true ? sfd.FileName : null);
    }

    public Task<string?> ShowFolderSelectDialogAsync(string Title, string InitPathName = "")
    {
        var dialog = new OpenFolderDialog()
        {
            DefaultDirectory = InitPathName,
            Title = Title
            
        };

        if (!dialog.ShowDialog(Active) == true)
            return Task.FromResult<string?>(null);

        return Task.FromResult(dialog.FolderName)!;
    }


    private static async Task DoDispatcheredAction(Task action) => await Application.Current.Dispatcher.Invoke(async () => await action, DispatcherPriority.Normal);


    private static async Task<T?> GetDispatcheredResult<T>(Func<Task<T>> action)
    {
        T? result = default;

        await Application.Current.Dispatcher.Invoke(async () => result = await action.Invoke(), DispatcherPriority.Normal);

        return result;
    }
}