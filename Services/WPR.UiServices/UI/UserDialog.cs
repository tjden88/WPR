using System.Windows;
using WPR.Dialogs;
using WPR.Domain.Dialogs;
using WPR.Interfaces.Base.UI;
using WPR.Interfaces.UI;
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
        await WPRDialogHelper.InformationAsync(Active, message, Title);


    public async Task<bool> QuestionAsync(string message, string? Title = null) =>
        await WPRDialogHelper.QuestionAsync(Active, message, Title);


    public async Task<bool?> QuestionAsync(string message, IUserDialog.DialogTypes DilaogType, string? Title = null) =>
        DilaogType switch
        {
            IUserDialog.DialogTypes.YesNo => await WPRDialogHelper.QuestionAsync(Active, message, Title),
            IUserDialog.DialogTypes.YesNoCancel => await WPRDialogHelper.QuestionCancelAsync(Active, message, Title),
            IUserDialog.DialogTypes.OkCancel => await WPRDialogHelper.InformationCancelAsync(Active, message, Title),
            _ => throw new ArgumentOutOfRangeException(nameof(DilaogType), DilaogType, null)
        };



    public async Task<bool?> CustomQuestionAsync(string message, string? Title, string TrueCaption, string? FalseCaption = null,
        string? NullCaption = null) =>
        await WPRDialogHelper.ShowCustomButtonsDialog(Active, message, Title, TrueCaption, FalseCaption,
            NullCaption);


    public async Task LoadingAsync(CancellationToken cancel = default)
    {
        throw new NotImplementedException();
    }



    public async Task ErrorMessageAsync(string message, string? Title = "Ошибка")
        => await WPRDialogHelper.ErrorAsync(Active, message, Title);


    public async Task<bool> CustomDialogAsync(IWPRDialog Dialog)
        => await WPRDialogHelper.ShowCustomDialogAsync(Active, Dialog);


    public async Task<string?> InputTextAsync(string title, string? DefaultValue = null, string? message = null) =>
        await WPRDialogHelper.InputTextAsync(Active, title, null, DefaultValue);

    public async Task<string?> InputValidatedTextAsync(InputDialogFilter DialogFilter) =>
        await WPRDialogHelper.InputTextAsync(Active,
            DialogFilter.Title,
            DialogFilter.Message,
            DialogFilter.DefaultValue,
            DialogFilter.ValidationRules.Select(f => new PredicateValidationRule<string>(f.Rule, f.ErrorMessage)));

    public Task ShowNotificationAsync(string message, int delay = 2000)
    {
        WPRDialogHelper.Bubble(Active, message, delay);
        return Task.CompletedTask;
    }

    public Task<bool> ShowQuestionNotificationAsync(string message, string AcceptCaption, int delay = 3000)
    {
        var source = new TaskCompletionSource<bool>();
        WPRDialogHelper.Bubble(Active, message, AcceptCaption, b => source.TrySetResult(b), delay);

        return source.Task;
    }
}