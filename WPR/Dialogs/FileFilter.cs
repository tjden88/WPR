namespace WPR.Dialogs;

/// <summary>
/// Фильтр файлов
/// </summary>
public class FileFilter
{
    private FileFilter()
    {
    }

    private static readonly string[] _ImagePatterns = ["*.jpg", "*.jpeg", "*.png", "*.gif", "*.bmp"];

    private readonly List<string> _FileMathPatterns = new();

    /// <summary>
    /// Список паттернов поиска совпадений файлов
    /// Пример паттерна: *.jpg
    /// </summary>
    public IEnumerable<string> FileMathPatterns => _FileMathPatterns;

    /// <summary> Описание фильтра </summary>
    public string Description { get; init; }


    public FileFilter(string Description, IEnumerable<string> fileMathPatterns)
    {
        this.Description = Description;
        _FileMathPatterns.AddRange(fileMathPatterns);
    }


    public void Deconstruct(out string description, out IEnumerable<string> fileMathPatterns)
    {
        fileMathPatterns = FileMathPatterns;
        description = Description;
    }

    #region Extensions

    /// <summary> Добавить паттерн поиска </summary>
    public FileFilter AddFileMathPattern(string extension)
    {
        if (!FileMathPatterns.Contains(extension))
            _FileMathPatterns.Add(extension);
        return this;
    }

    /// <summary> Добавить паттерны поиска изображений </summary>
    public FileFilter AddImagesMathPatterns()
    {
        var unique = _ImagePatterns
            .Where(ip => !_FileMathPatterns.Contains(ip));
        _FileMathPatterns.AddRange(unique);
        return this;
    }

    /// <summary> Объединить с другим фильтром </summary>
    public List<FileFilter> Union(FileFilter other) => [other, this];

    /// <summary> Превратить в список </summary>
    public List<FileFilter> ToList() => [this];


    #endregion


    #region Factory

    /// <summary>Создать фильтр с одним паттерном </summary>
    public static FileFilter CreateSingleExtension(string Description, string FileMathPattern) =>
        new(Description, [FileMathPattern]);

    /// <summary> Создать фильтр для поиска всех файлов </summary>
    public static FileFilter CreateAllFilesFilter() => new("Все файлы", ["*.*"]);


    /// <summary> Создать фильтр для поиска изображений </summary>
    public static FileFilter CreateImagesFilter() => new("Изображения", _ImagePatterns);


    #endregion


}
