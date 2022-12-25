using WPR.Dialogs;
using WPR.Interfaces.Base.UI;
using WPR.Interfaces.UI;

namespace WPR.UiServices.UI;

public class UserDialog : IUserDialog
{


    public UserDialog()
    {
        
    }

    public async Task InformationAsync(string message, string? Title = null)
    {
        return await WPRDialogHelper.InformationAsync()
    }

    public async Task<bool> QuestionAsync(string message, string? Title = null)
    {
        throw new NotImplementedException();
    }

    public async Task<bool?> QuestionAsync(string message, IUserDialog.DialogTypes DilaogType, string? Title = null)
    {
        throw new NotImplementedException();
    }

    public async Task<bool?> CustomQuestionAsync(string message, string? Title, string TrueCaption, string? FalseCaption = null,
        string? NullCaption = null)
    {
        throw new NotImplementedException();
    }

    public async Task LoadingAsync(CancellationToken cancel = default)
    {
        throw new NotImplementedException();
    }

    public async Task ErrorMessageAsync(string message, string? Title = "Ошибка")
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CustomDialogAsync(IWPRDialog Dialog)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool Result, string Text)> InputTextAsync(string message, string? DefaultValue = null, string? Title = null)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool Result, string Text)> InputValidatedTextAsync(Predicate<string> ValidationRule, string message, string? DefaultValue = null,
        string? Title = null)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool Result, string Text)> InputValidatedTextAsync(IEnumerable<Predicate<string>> ValidationRules, string message, string? DefaultValue = null,
        string? Title = null)
    {
        throw new NotImplementedException();
    }
}