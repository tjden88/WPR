using Microsoft.Win32;
using System.Windows;
using System.Windows.Threading;
using WPR.Dialogs;
using WPR.Domain.Interfaces;
using WPR.Domain.Models.Dialogs;
using WPR.MVVM.Validation;
using WPR.UiServices.Interfaces;

namespace WPR.UiServices.UI;

public class UserDialog : IUserDialog
{
    private readonly IAppNavigation _AppNavigation;

    private Window? Active => _AppNavigation.ActiveWindow;

    public UserDialog(IAppNavigation AppNavigation)
    {
        _AppNavigation = AppNavigation;
    }

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



    public async Task ErrorMessageAsync(string message, string? Title = "Ошибка")
        => await DoDispatcheredAction(WPRDialogHelper.ErrorAsync(Active, message, Title));


    public async Task<bool> CustomDialogAsync(IWPRDialog Dialog)
        => await GetDispatcheredResult(() => WPRDialogHelper.ShowCustomDialogAsync(Active, Dialog));


    public async Task<string?> InputTextAsync(string title, string? DefaultValue = null, string? message = null) =>
        await GetDispatcheredResult(() => WPRDialogHelper.InputTextAsync(Active, title, null, DefaultValue));

    public async Task<string?> InputValidatedTextAsync(InputDialogFilter DialogFilter) =>
        await GetDispatcheredResult(() => WPRDialogHelper.InputTextAsync(Active,
            DialogFilter.Title,
            DialogFilter.Message,
            DialogFilter.DefaultValue,
            DialogFilter.ValidationRules.Select(f => new PredicateValidationRule<string>(f.Rule, f.ErrorMessage))));

    public Task ShowNotificationAsync(string message, int delay = 2000)
    {
        Application.Current.Dispatcher.Invoke(() => WPRDialogHelper.Bubble(Active, message, delay));
        return Task.CompletedTask;
    }

    public async Task<bool> ShowQuestionNotificationAsync(string message, string AcceptCaption, int delay = 3000)
    {
        return await GetDispatcheredResult(() =>
        {
            var result = new TaskCompletionSource<bool>();
            WPRDialogHelper.Bubble(Active, message, AcceptCaption, b => result.TrySetResult(b), delay);
            return result.Task;
        });
    }

    public Task<string?> ShowOpenFileDialogAsync(string Title, IEnumerable<IFileFilter>? Filters = null, string InitFileName = "")
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

    public Task<string?> ShowSaveFileDialogAsync(string Title, IEnumerable<IFileFilter>? Filters = null, string InitFileName = "")
    {
        throw new NotImplementedException();
    }

    public Task<string?> ShowFolderSelectDialogAsync(string Title, string InitPathName = "")
    {
        throw new NotImplementedException();
    }


    private static async Task DoDispatcheredAction(Task action) => await Application.Current.Dispatcher.Invoke(async () => await action, DispatcherPriority.Normal);


    private static async Task<T?> GetDispatcheredResult<T>(Func<Task<T>> action)
    {
        T? result = default;

        await Application.Current.Dispatcher.Invoke(async () => result = await action.Invoke(), DispatcherPriority.Normal);

        return result;
    }
}