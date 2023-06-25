namespace WPR.UiServices.Interfaces;

/// <summary> Шина обмена сообщениями </summary>
public interface IMessageBus
{
    /// <summary> Зарегистрировать обработчик события </summary>
    IDisposable RegisterHandler<T>(Action<T> Handler);

    /// <summary> Отправить сообщение </summary>
    void Send<T>(T message);
}
