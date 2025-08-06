using WPR.Theme;

namespace WPR.Dialogs;

/// <summary>
/// Сервис диалоговых окон с пользователем
/// </summary>
public interface IUserDialog
{

    /// <summary> Показать информационное сообщение </summary>
    Task InformationAsync(string message, string Title = null);


    /// <summary> Вопрос с вариантами ДА, НЕТ </summary>
    Task<bool> QuestionAsync(string message, string Title = null);


    /// <summary> Вопрос с расширенными вариантами ответов </summary>
    Task<bool?> QuestionAsync(string message, DialogType dialogType, string Title = null);


    /// <summary>
    /// Диалог с другими вариантами ответа
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="Title">Заголовок</param>
    /// <param name="AcceptCaption">Подпись на подтверждающей кнопке</param>
    /// <param name="RejectCaption">Подпись на кнопке отказа (не будет показана, если null)</param>
    /// <param name="CancelCaption">Подпись на кнопке отмены (не будет показана, если null)</param>
    /// <returns>Результат в соответствии с выбранной кнопкой</returns>
    Task<bool?> CustomQuestionAsync(string message, string Title, string AcceptCaption, string RejectCaption = null, string CancelCaption = null);


    /// <summary> Предупреждение об ошибке </summary>
    Task ErrorMessageAsync(string message, string Title = "Ошибка");


    /// <summary> Показать произвольный диалог </summary>
    Task<bool> CustomDialogAsync(IWPRDialog Dialog);


    /// <summary> Текстовое поле для ввода. Если null - пользователь отменил ввод </summary>
    Task<string> InputTextAsync(string title, string DefaultValue = null, string message = null, bool MultiLine = false);


    /// <summary> Текстовое поле для ввода с валидацией введённых данных. Если null - пользователь отменил ввод </summary>
    Task<string> InputValidatedTextAsync(Action<InputDialogFilterOptions> options);


    /// <summary> Показать всплывающее уведомление </summary>
    Task ShowNotificationAsync(string message, StyleBrushes background = StyleBrushes.BackgroundContrastColorBrush, int delay = 2000);


    /// <summary>
    /// Показать всплывающее уведомление с ожиданием реакции пользователя
    /// </summary>
    /// <param name="message">Сообщение пользователю</param>
    /// <param name="AcceptCaption">Подпись кнопки подтверждения</param>
    /// <param name="delay">Время показа уведомления</param>
    /// <param name="background">Фон сообщения</param>
    /// <returns>True, если пользователь нажал кнопку подтверждения</returns>
    Task<bool> ShowQuestionNotificationAsync(string message, string AcceptCaption, StyleBrushes background = StyleBrushes.BackgroundContrastColorBrush, int delay = 3000);


    /// <summary>
    /// Показать диалог выбора файла
    /// </summary>
    /// <param name="Title">Заголовок</param>
    /// <param name="Filters">Список фильтров файлов, доступных для выбора</param>
    /// <param name="InitFileName">Начальное имя файла</param>
    /// <returns> Null - пользователь отказался
    /// </returns>
    Task<string> ShowOpenFileDialogAsync(string Title, IEnumerable<FileFilter> Filters = null, string InitFileName = "");


    /// <summary>
    /// Показать диалог сохранения файла
    /// </summary>
    /// <param name="Title">Заголовок</param>
    /// <param name="Filters">Список фильтров файлов, доступных для выбора</param>
    /// <param name="InitFileName">Начальное имя файла</param>
    /// <returns>
    /// Null - пользователь отказался
    /// </returns>
    Task<string> ShowSaveFileDialogAsync(string Title, IEnumerable<FileFilter> Filters = null, string InitFileName = "");


    /// <summary>
    /// Показать диалог выбора директории
    /// </summary>
    /// <param name="Title">Заголовок</param>
    /// <param name="InitPathName">Начальное имя директории</param>
    /// <returns>
    /// Null - пользователь отказался
    /// </returns>
    Task<string> ShowFolderSelectDialogAsync(string Title, string InitPathName = "");
}