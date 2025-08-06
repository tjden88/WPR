using System.Windows;
using System.Windows.Input;
using WPR.Infrastructure.Commands;

namespace WPR.Dialogs;

public class MessageDialog : DialogBase
{
    public enum DialogTypes
    {
        Information,
        Question,
        InformationCancel,
        QuestionCancel,
    }

    /// <summary>
    /// Получает значение true, если диалог был отменён кнопкой "Отмена".
    /// </summary>
    public bool IsCancelled { get; private set; }


    static MessageDialog()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialog), new FrameworkPropertyMetadata(typeof(MessageDialog)));
    }

    public MessageDialog()
    {
        CancelButtonPressedCommand = new BaseCommand(() =>
        {
            IsCancelled = true;
            RaiseCompleted(false);
        });
    }


    public ICommand CancelButtonPressedCommand { get; }


    #region Init Props

    public DialogTypes DialogType { get; init; } = DialogTypes.Information;

    public bool IsErrorMessage { get; init; }

    public string AcceptButtonText { get; init; } = "OK";
    public string CancelButtonText { get; init; } = "Отмена";

    public string QuestionAcceptButtonText { get; init; } = "Да";
    public string QuestionCancelButtonText { get; init; } = "Нет";


    #endregion


    #region ButtonsVisibility

    public bool IsCancelButtonVisible => DialogType is DialogTypes.InformationCancel or DialogTypes.QuestionCancel;

    public bool IsQuestionButtonsVisible => DialogType is DialogTypes.Question or DialogTypes.QuestionCancel;

    public bool IsAcceptButtonVisible => DialogType is DialogTypes.Information or DialogTypes.InformationCancel;


    #endregion


}