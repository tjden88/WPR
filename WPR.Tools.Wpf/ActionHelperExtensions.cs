using WPR.Dialogs;
using WPR.Theme;

namespace WPR.Tools.Wpf;

/// <summary>
/// Тип сообщения для стилизации уведомлений.
/// </summary>
public enum MessageType
{
    /// <summary>Стандартное сообщение</summary>
    Default,
    /// <summary>Сообщение об ошибке (опасное действие)</summary>
    Danger,
    /// <summary>Сообщение об успешном выполнении</summary>
    Success
}


/// <summary>
/// Расширения для <see cref="ActionBuilder"/> и <see cref="ActionBuilder{T}"/>,
/// добавляющие методы для отображения сообщений и уведомлений.
/// </summary>
public static class ActionHelperExtensions
{

    private static readonly IUserDialog _Dialog = DialogHelper.Default;

    #region ActionBuilder

    /// <summary>
    /// Добавляет шаг для отображения диалогового сообщения (ошибка или информация).
    /// </summary>
    /// <param name="builder">Экземпляр <see cref="ActionBuilder"/>.</param>
    /// <param name="message">Текст сообщения.</param>
    /// <param name="title">Заголовок сообщения.</param>
    /// <param name="IsError">Если true — отображается как ошибка, иначе как информационное сообщение.</param>
    /// <returns>Тот же экземпляр <see cref="ActionBuilder"/> для цепочки вызовов.</returns>
    public static ActionBuilder ShowMessage(this ActionBuilder builder,
        string message,
        string title,
        bool IsError)
    {
        builder.Then(token => IsError
            ? _Dialog.ErrorMessageAsync(message, title, token)
            : _Dialog.InformationAsync(message, title, token));

        return builder;
    }

    /// <summary>
    /// Добавляет шаг для отображения диалога с ошибкой.
    /// </summary>
    /// <param name="builder">Экземпляр <see cref="ActionBuilder"/>.</param>
    /// <param name="message">Текст ошибки.</param>
    /// <returns>Тот же экземпляр <see cref="ActionBuilder"/> для цепочки вызовов.</returns>
    public static ActionBuilder ShowError(this ActionBuilder builder, string message) =>
        ShowMessage(builder, message, "Ошибка", true);

    /// <summary>
    /// Добавляет шаг для отображения информационного диалога (без заголовка).
    /// </summary>
    /// <param name="builder">Экземпляр <see cref="ActionBuilder"/>.</param>
    /// <param name="message">Текст сообщения.</param>
    /// <returns>Тот же экземпляр <see cref="ActionBuilder"/> для цепочки вызовов.</returns>
    public static ActionBuilder ShowInfo(this ActionBuilder builder, string message) =>
        ShowMessage(builder, message, "", false);

    /// <summary>
    /// Добавляет шаг для отображения всплывающего уведомления с настраиваемым стилем.
    /// </summary>
    /// <param name="builder">Экземпляр <see cref="ActionBuilder"/>.</param>
    /// <param name="message">Текст уведомления.</param>
    /// <param name="messageType">Тип сообщения (влияет на цвет).</param>
    /// <returns>Тот же экземпляр <see cref="ActionBuilder"/> для цепочки вызовов.</returns>
    public static ActionBuilder ShowNotification(this ActionBuilder builder, string message,
        MessageType messageType = MessageType.Default)
    {
        var color = messageType switch
        {
            MessageType.Default => StyleBrushes.BackgroundContrastColorBrush,
            MessageType.Danger => StyleBrushes.DangerColorBrush,
            MessageType.Success => StyleBrushes.SuccessColorBrush,
            _ => StyleBrushes.BackgroundContrastColorBrush,
        };

        builder.Then(_ => _Dialog.ShowNotificationAsync(message, color));
        return builder;
    } 
    #endregion

    #region ActionBuilder<T>

    /// <summary>
    /// Регистрирует обработчик неуспешного выполнения шага, который показывает ошибку в диалоге.
    /// </summary>
    /// <typeparam name="T">Тип результата шага.</typeparam>
    /// <param name="builder">Экземпляр <see cref="ActionBuilder{T}"/>.</param>
    /// <param name="message">Функция, возвращающая текст ошибки на основе результата шага.</param>
    /// <returns>Тот же экземпляр <see cref="ActionBuilder{T}"/> для цепочки вызовов.</returns>
    public static ActionBuilder<T> OnFailErrorMessage<T>(this ActionBuilder<T> builder, Func<T, string> message)
    {
        builder.OnFail(async (value, token) =>
        {
            var msg = message.Invoke(value);
            await _Dialog.ErrorMessageAsync(msg, cancellationToken: token);
        });

        return builder;
    }

    /// <summary>
    /// Регистрирует обработчик неуспешного выполнения шага, который показывает уведомление об ошибке.
    /// </summary>
    /// <typeparam name="T">Тип результата шага.</typeparam>
    /// <param name="builder">Экземпляр <see cref="ActionBuilder{T}"/>.</param>
    /// <param name="message">Функция, возвращающая текст уведомления на основе результата шага.</param>
    /// <returns>Тот же экземпляр <see cref="ActionBuilder{T}"/> для цепочки вызовов.</returns>
    public static ActionBuilder<T> OnFailErrorNotification<T>(this ActionBuilder<T> builder, Func<T, string> message)
    {
        builder.OnFail(async (value, _) =>
        {
            var msg = message.Invoke(value);
            await _Dialog.ShowNotificationAsync(msg, StyleBrushes.DangerColorBrush);
        });

        return builder;
    }

    /// <summary>
    /// Регистрирует обработчик успешного выполнения шага, который показывает информационное сообщение.
    /// </summary>
    /// <typeparam name="T">Тип результата шага.</typeparam>
    /// <param name="builder">Экземпляр <see cref="ActionBuilder{T}"/>.</param>
    /// <param name="message">Функция, возвращающая текст сообщения на основе результата шага.</param>
    /// <returns>Тот же экземпляр <see cref="ActionBuilder{T}"/> для цепочки вызовов.</returns>
    public static ActionBuilder<T> OnSuccessInfo<T>(this ActionBuilder<T> builder, Func<T, string> message)
    {
        builder.OnSuccess((value, token) =>
        {
            var msg = message.Invoke(value);
            return _Dialog.InformationAsync(msg, cancellationToken: token);
        });

        return builder;
    }

    /// <summary>
    /// Регистрирует обработчик успешного выполнения шага, который показывает уведомление.
    /// </summary>
    /// <typeparam name="T">Тип результата шага.</typeparam>
    /// <param name="builder">Экземпляр <see cref="ActionBuilder{T}"/>.</param>
    /// <param name="message">Функция, возвращающая текст уведомления на основе результата шага.</param>
    /// <returns>Тот же экземпляр <see cref="ActionBuilder{T}"/> для цепочки вызовов.</returns>
    public static ActionBuilder<T> OnSuccessNotification<T>(this ActionBuilder<T> builder, Func<T, string> message)
    {
        builder.OnSuccess((value, _) =>
        {
            var msg = message.Invoke(value);
            return _Dialog.ShowNotificationAsync(msg, StyleBrushes.SuccessColorBrush);
        });

        return builder;
    }
    #endregion

    #region CheckActionBuilder

    /// <summary>
    /// Регистрирует обработчик неуспешного выполнения шага проверки, который показывает ошибку в диалоге.
    /// </summary>
    /// <param name="builder">Экземпляр <see cref="CheckActionBuilder"/>.</param>
    /// <param name="message">Текст ошибки</param>
    /// <returns>Тот же экземпляр <see cref="CheckActionBuilder"/> для цепочки вызовов.</returns>
    public static CheckActionBuilder OnFailErrorMessage(this CheckActionBuilder builder, string message)
    {
        builder.OnFail(ct => _Dialog.ErrorMessageAsync(message, cancellationToken: ct));
        return builder;
    }

    /// <summary>
    /// Регистрирует обработчик неуспешного выполнения шага проверки, который показывает уведомление об ошибке.
    /// </summary>
    /// <param name="builder">Экземпляр <see cref="CheckActionBuilder"/>.</param>
    /// <param name="message">Текст ошибки</param>
    /// <returns>Тот же экземпляр <see cref="CheckActionBuilder"/> для цепочки вызовов.</returns>
    public static CheckActionBuilder OnFailErrorNotification(this CheckActionBuilder builder, string message)
    {
        builder.OnFail(_ => _Dialog.ShowNotificationAsync(message, StyleBrushes.DangerColorBrush));
        return builder;
    }

    /// <summary>
    /// Регистрирует обработчик успешного выполнения шага проверки, который показывает информационный диалог.
    /// </summary>
    /// <param name="builder">Экземпляр <see cref="CheckActionBuilder"/>.</param>
    /// <param name="message">Текст ошибки</param>
    /// <returns>Тот же экземпляр <see cref="CheckActionBuilder"/> для цепочки вызовов.</returns>
    public static CheckActionBuilder OnSuccessInfo(this CheckActionBuilder builder, string message)
    {
        builder.OnSuccess(ct => _Dialog.InformationAsync(message, cancellationToken: ct));
        return builder;
    }

    /// <summary>
    /// Регистрирует обработчик успешного выполнения шага проверки, который показывает информационное уведомление.
    /// </summary>
    /// <param name="builder">Экземпляр <see cref="CheckActionBuilder"/>.</param>
    /// <param name="message">Текст ошибки</param>
    /// <returns>Тот же экземпляр <see cref="CheckActionBuilder"/> для цепочки вызовов.</returns>
    public static CheckActionBuilder OnSuccessNotification(this CheckActionBuilder builder, string message)
    {
        builder.OnSuccess(_ => _Dialog.ShowNotificationAsync(message, StyleBrushes.SuccessColorBrush));
        return builder;
    } 
    #endregion
}