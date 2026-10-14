using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace WPR.Operations;

/// <summary>
/// Универсальная обертка для операций, возвращающих значение типа <typeparamref name="T"/>,
/// с возможностью указания ошибки типа <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="T">Тип значения</typeparam>
/// <typeparam name="TError">Тип ошибки</typeparam>
/// <remarks>
/// Для <typeparamref name="TError"/> не существует универсального «пустого» значения,
/// поэтому признак успеха хранится в отдельном явном флаге <see cref="IsSuccess"/>.
/// Флаг выставляется только фабриками <see cref="Success"/> и <see cref="Failure"/>,
/// а <c>default(OperationResult&lt;T, TError&gt;)</c> даёт неудачу (fail-safe).
/// </remarks>
public readonly struct OperationResult<T, TError> : IOperationResult<TError>
{
    private OperationResult(T? value, TError error, bool isSuccess)
    {
        Value = value;
        Error = error;
        IsSuccess = isSuccess;
    }

    /// <summary>
    /// Значение операции, если она успешна.
    /// </summary>
    public T? Value { get; init; }

    /// <summary>
    /// Ошибка, если операция завершилась неудачей.
    /// </summary>
    public TError Error { get; init; }

    /// <summary>
    /// Указывает, что операция завершилась успешно.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Указывает, что операция завершилась неудачей.
    /// </summary>
    [JsonIgnore]
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Создает успешную операцию с заданным значением.
    /// </summary>
    /// <param name="value">Значение операции.</param>
    public static OperationResult<T, TError> Success(T value) =>
        new(value, default!, true);

    /// <summary>
    /// Создает неудачный результат операции с заданной ошибкой.
    /// </summary>
    /// <param name="error">
    /// Ошибка. Для ссылочных и nullable-типов не может быть <see langword="null"/>,
    /// иначе «неудача» была бы неотличима от успеха.
    /// </param>
    /// <exception cref="ArgumentNullException">Ошибка равна <see langword="null"/>.</exception>
    public static OperationResult<T, TError> Failure(TError error)
    {
        if (error is null)
            throw new ArgumentNullException(nameof(error), "Ошибка не может быть null");

        return new(default, error, false);
    }

    /// <summary>
    /// Деконструирует результат на значение и ошибку.
    /// </summary>
    public void Deconstruct(out T? value, out TError? error)
    {
        value = Value;
        error = Error;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return IsSuccess
            ? $"Success ({Value})"
            : $"Failure ({Error})";
    }

    /// <summary>
    /// Получает значение операции, если она успешна, или выбрасывает исключение, если неудачна.
    /// </summary>
    /// <returns>Значение успешной операции.</returns>
    /// <exception cref="InvalidOperationException">Операция завершилась неудачей.</exception>
    public T GetValueOrThrow()
    {
        if (!IsSuccess)
            throw new InvalidOperationException($"Operation failed: {Error}");

        return Value!;
    }

    /// <summary>
    /// Получает значение операции, если она успешна, или возвращает значение по умолчанию, если неудачна.
    /// </summary>
    /// <param name="fallback">Значение, возвращаемое при неудаче.</param>
    public T? GetValueOrDefault(T? fallback = default) => IsSuccess ? Value : fallback;

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

        value = default!;
        return false;
    }

    #region Operators

    /// <summary>
    /// Неявно создает успешный результат из значения.
    /// </summary>
    public static implicit operator OperationResult<T, TError>(T value) => Success(value);

    /// <summary>
    /// Явное извлечение значения операции.
    /// Намеренно объявлено <c>explicit</c>, чтобы неудача не «превращалась» молча в <c>default</c>.
    /// </summary>
    public static explicit operator T?(OperationResult<T, TError> op) => op.Value;

    /// <summary>
    /// Неявно превращает результат в признак успеха (для <c>if (result)</c>).
    /// </summary>
    public static implicit operator bool(OperationResult<T, TError> result) => result.IsSuccess;

    #endregion
}
