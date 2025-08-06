namespace WPR.Repository.Abstractions;


/// <summary>
/// Тип регистрируемого репозитория для расширений IServiceCollection
/// </summary>
public enum RegisterMethod
{
    /// <summary> Только IRepository </summary>
    CrudRepository,

    /// <summary> Только ReadOnlyRepository </summary>
    ReadOnlyRepository,

    /// <summary> Оба варианта </summary>
    Both

}