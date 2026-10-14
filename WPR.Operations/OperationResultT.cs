using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace WPR.Operations;

/// <summary>
/// Универсальная обертка для операций, возвращающих значение типа <typeparamref name="T"/>,
/// с возможностью указания ошибки.
/// </summary>
/// <typeparam name="T">Тип значения операции.</typeparam>
/// <remarks>
/// Источник истины об успехе — <see cref="Error"/>: успешная операция содержит пустую строку,
/// любое другое значение (в том числе <see langword="null"/> из <c>default(OperationResult&lt;T&gt;)</c>)
/// трактуется как неудача. Это делает <c>default</c> «безопасным» (неудача) и исключает
/// рассогласование состояния при объектной инициализации, <c>with</c> или десериализации JSON.
/// </remarks>
public readonly struct OperationResult<T> : IOperationResult<string>
{
    private OperationResult(T value, string error)
    {
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Значение операции, если она успешна.
    /// </summary>
    public T Value { get; init; }

    /// <summary>
    /// Сообщение об ошибке, если операция завершилась неудачей, либо пустая строка при успехе.
    /// </summary>
    public string Error { get; init; }

    /// <summary>
    /// Указывает, что операция завершилась успешно.
    /// </summary>
    public bool IsSuccess => Error is { Length: 0 };

    /// <summary>
    /// Указывает, что операция завершилась неудачей.
    /// </summary>
    [JsonIgnore]
    public bool IsFailure => !IsSuccess;

    #region Factory

    /// <summary>
    /// Создает успешную операцию с заданным значением.
    /// </summary>
    /// <param name="value">
    /// Значение операции. Для ссылочных и nullable-типов допускается <see langword="null"/>.
    /// </param>
    public static OperationResult<T> Success(T value) => new(value, string.Empty);

    /// <summary>
    /// Создает неудачный результат операции с заданным сообщением об ошибке.
    /// </summary>
    /// <param name="error">Текст ошибки. Не может быть пустым или состоять только из пробелов.</param>
    /// <exception cref="ArgumentException">Сообщение об ошибке пустое или состоит только из пробелов.</exception>
    public static OperationResult<T> Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Сообщение об ошибке не может быть пустым", nameof(error));

        return new OperationResult<T>(default!, error);
    }

    #endregion

    #region Operators

    /// <summary>
    /// Неявно создает успешный результат из значения.
    /// </summary>
    public static implicit operator OperationResult<T>(T value) => Success(value);

    /// <summary>
    /// Явное извлечение значения операции.
    /// Намеренно объявлено <c>explicit</c>: неявное преобразование молча возвращало бы
    /// <c>default</c> при неудаче и скрывало бы ошибку. Используйте
    /// <see cref="GetValueOrThrow"/>, <see cref="GetValueOrDefault"/> или <see cref="TryGetValue(out T)"/>.
    /// </summary>
    public static explicit operator T(OperationResult<T> op) => op.Value;

    /// <summary>
    /// Неявно превращает результат в признак успеха (для <c>if (operation)</c>).
    /// </summary>
    public static implicit operator bool(OperationResult<T> op) => op.IsSuccess;

    #endregion

    /// <summary>
    /// Деконструирует результат на значение и текст ошибки.
    /// На успешном результате <paramref name="error"/> равна пустой строке.
    /// </summary>
    public void Deconstruct(out T? value, out string? error)
    {
        value = Value;
        error = Error;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return IsSuccess
            ? $"Success ({Value})"
            : $"Failure (\"{Error}\")";
    }

    /// <summary>
    /// Получает значение операции, если она успешна, или выбрасывает исключение, если неудачна.
    /// </summary>
    /// <returns>Значение успешной операции.</returns>
    /// <exception cref="InvalidOperationException">Операция завершилась неудачей.</exception>
    public T GetValueOrThrow()
    {
        if (!IsSuccess)
            throw new InvalidOperationException($"Операция завершилась неудачей: {Error}");

        return Value!;
    }

    /// <summary>
    /// Получает значение операции, если она успешна, или возвращает значение по умолчанию, если неудачна.
    /// </summary>
    /// <param name="fallback">Значение, возвращаемое при неудаче.</param>
    public T GetValueOrDefault(T fallback = default!) => IsSuccess ? Value : fallback;

    /// <summary>
    /// Пытается получить значение успешной операции без выброса исключения.
    /// </summary>
    /// <param name="value">Значение операции при успехе, иначе <c>default</c>.</param>
    /// <returns><see langword="true"/>, если операция успешна; иначе <see langword="false"/>.</returns>
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        if (IsSuccess)
        {
            value = Value!;
            return true;
        }

        value = default;
        return false;
    }
}
