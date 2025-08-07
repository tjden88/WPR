namespace WPR.Mvvm.Interfaces;

/// <summary>
/// Доступ к сервисам приложения, для отложенной или предотвращения циклической зависимости.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IResolver<out T> where T : notnull
{
    T Get();

    //IDisposable GetScope(out T value); - на будущее, если понадобится поддержка
}