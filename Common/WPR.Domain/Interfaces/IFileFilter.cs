namespace WPR.Domain.Interfaces
{
    /// <summary>
    /// Интерфейс фильтра файлов
    /// </summary>
    public interface IFileFilter
    {
        /// <summary> Расширение файла </summary>
        IEnumerable<string> Extensions { get; }

        /// <summary> Описание типа файла </summary>
        string Description { get; }

    }
}
