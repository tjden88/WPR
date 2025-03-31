using WPR.Abstractions.Interfaces;

namespace WPR.Abstractions.Models.Files;

/// <summary>
/// Фильтр файлов
/// </summary>
public class FileFilter
{
    private static readonly string[] _ImagePatterns = {"*.jpg", "*.jpeg", "*.png", "*.gif", "*.bmp",};

    private readonly List<string> _FileMathPattrerns = new();

    /// <summary>
    /// Список паттернов поиска совпадений файлов
    /// Пример паттерна: *.jpg
    /// </summary>
    public IEnumerable<string> FileMathPattrerns => _FileMathPattrerns;

    /// <summary> Описание фильтра </summary>
    public string Description { get; init; }


    public FileFilter(string Description, IEnumerable<string> FileMathPattrerns)
    {
        this.Description = Description;
        _FileMathPattrerns.AddRange(FileMathPattrerns);
    }


    public void Deconstruct(out string description, out IEnumerable<string> fileMathPattrerns)
    {
        fileMathPattrerns = FileMathPattrerns;
        description = Description;
    }

    #region Extensions

    /// <summary> Добавить паттерн поиска </summary>
    public FileFilter AddFileMathPattrern(string extension)
    {
        if (!FileMathPattrerns.Contains(extension))
            _FileMathPattrerns.Add(extension);
        return this;
    }

    /// <summary> Добавить паттерны поиска изображений </summary>
    public FileFilter AddImagesMathPattrerns()
    {
        var unique = _ImagePatterns
            .Where(ip => !_FileMathPattrerns.Contains(ip));
        _FileMathPattrerns.AddRange(unique);
        return this;
    }

    /// <summary> Объединить с другим фильтром </summary>
    public List<FileFilter> Union(FileFilter other) => new() {other, this};

    /// <summary> Превратить в список </summary>
    public List<FileFilter> ToList() => new() { this };


    #endregion


    #region Factory

    /// <summary>Создать фильтр с одним паттерном </summary>
    public static FileFilter CreateSingleExtension(string Description, string FileMathPattrern) =>
        new(Description, new[] { FileMathPattrern });

    /// <summary> Создать фильтр для поиска всех файлов </summary>
    public static FileFilter CreateAllFilesFilter() => new("Все файлы", new[] {"*.*"});


    /// <summary> Создать фильтр для поиска изображений </summary>
    public static FileFilter CreateImagesFilter() => new("Изображения", _ImagePatterns);


    #endregion


}
