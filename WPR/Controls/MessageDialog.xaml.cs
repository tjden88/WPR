using System.Windows;
using System.Windows.Input;
using WPR.Controls.Base;
using WPR.Dialogs;
using WPR.Infrastructure.Commands;

namespace WPR;

public class MessageDialog : DialogBase
{

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

    public DialogType DialogType { get; init; } = DialogType.Information;

    public bool IsErrorMessage { get; init; }

    public string AcceptButtonText { get; init; } = "OK";
    public string CancelButtonText { get; init; } = "Отмена";

    public string QuestionAcceptButtonText { get; init; } = "Да";
    public string QuestionCancelButtonText { get; init; } = "Нет";


    #endregion


    #region ButtonsVisibility

    public bool IsCancelButtonVisible => DialogType is DialogType.InformationCancel or DialogType.QuestionCancel;

    public bool IsQuestionButtonsVisible => DialogType is DialogType.Question or DialogType.QuestionCancel;

    public bool IsAcceptButtonVisible => DialogType is DialogType.Information or DialogType.InformationCancel;


    #endregion


}