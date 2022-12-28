using WPR.Domain.Dialogs;
using WPR.Interfaces.Base.UI;

namespace WPR.Interfaces.UI;

/// <summary>
/// Сервис диалоговых окон с пользователем
/// </summary>
public interface IUserDialog
{
    /// <summary> Типы вариантов диалогов </summary>
    public enum DialogTypes
    {
        YesNo,
        YesNoCancel,
        OkCancel,
    }

    /// <summary> Показать информационное сообщение </summary>
    Task InformationAsync(string message, string? Title = null);


    /// <summary> Вопрос с вариантами ДА, НЕТ </summary>
    Task<bool> QuestionAsync(string message, string? Title = null);


    /// <summary> Вопрос с расширенными вариантами ответов </summary>
    Task<bool?> QuestionAsync(string message, DialogTypes DilaogType, string? Title = null);


    /// <summary>
    /// Диалог с другими вариантами ответа
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="Title">Заголовок</param>
    /// <param name="TrueCaption">Подпись на подтверждающей кнопке</param>
    /// <param name="FalseCaption">Подпись на кнопке отказа (не будет показана, если null)</param>
    /// <param name="NullCaption">Подпись на кнопке отмены (не будет показана, если null)</param>
    /// <returns>Результат в соответствии с выбранной кнопкой</returns>
    Task<bool?> CustomQuestionAsync(string message, string? Title, string TrueCaption, string? FalseCaption = null, string? NullCaption = null);


    /// <summary> Показать индикатор загрузки </summary>
    Task LoadingAsync(CancellationToken cancel = default);


    /// <summary> Предупреждение об ошибке </summary>
    Task ErrorMessageAsync(string message, string? Title = "Ошибка");


    /// <summary> Показать произвольный диалог и дождаться результата </summary>
    Task<bool> CustomDialogAsync(IWPRDialog Dialog);


    /// <summary> Текстовое поле для ввода. Если null - пользователь отменил ввод </summary>
    Task<string?> InputTextAsync(string title, string? DefaultValue = null, string? message = null);


    /// <summary> Текстовое поле для ввода с валидацией введённых данных. Если null - пользователь отменил ввод </summary>
    Task<string?> InputValidatedTextAsync(InputDialogFilter DialogFilter);


    /// <summary> Показать всплывающее уведомление </summary>
    Task ShowNotificationAsync(string message, int delay = 2000);


    /// <summary>
    /// Показать всплывающее уведомление с ожиданием реакции пользователя
    /// </summary>
    /// <param name="message">Сообщение пользователю</param>
    /// <param name="AcceptCaption">Подпись кнопки подтверждения</param>
    /// <param name="delay">Время показа уведомления</param>
    /// <returns></returns>
    Task<bool> ShowQuestionNotificationAsync(string message, string AcceptCaption, int delay = 3000);
}