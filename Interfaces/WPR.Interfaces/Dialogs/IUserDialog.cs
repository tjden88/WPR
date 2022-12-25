using WPR.Interfaces.Base.UI;

namespace WPR.Interfaces.Dialogs;

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


    /// <summary> Текстовое поле для ввода </summary>
    Task<(bool Result, string Text)> InputTextAsync(string message, string? DefaultValue = null, string? Title = null);


    /// <summary> Текстовое поле для ввода с валидацией введённых данных</summary>
    Task<(bool Result, string Text)> InputValidatedTextAsync(Predicate<string> ValidationRule, string message, string? DefaultValue = null, string? Title = null);


    /// <summary> Текстовое поле для ввода с валидацией введённых данных</summary>
    Task<(bool Result, string Text)> InputValidatedTextAsync(IEnumerable<Predicate<string>> ValidationRules, string message, string? DefaultValue = null, string? Title = null);

}