using WPR.Domain.Interfaces;

namespace WPR.Domain.Models.Files;

/// <summary>
/// Фильтр файлов
/// </summary>
public class FileFilter : IFileFilter
{
    private static readonly string[] _ImagePatterns = {"*.jpg", "*.jpeg", "*.png", "*.gif", "*.bmp",};

    private readonly List<string> _FileMathPattrerns = new();
    public IEnumerable<string> FileMathPattrerns => _FileMathPattrerns;
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

    #endregion


    #region Factory

    /// <summary>Создать фильтр с одним паттерном </summary>
    public static FileFilter CreateSingleExtension(string Description, string FileMathPattrern) =>
        new(Description, new[] { FileMathPattrern });

    /// <summary> Создать фильтр для поиска всех файлов </summary>
    public static FileFilter AllFilesFilter() => new("Все файлы", new[] {"*.*"});

    public static FileFilter ImagesFilter() => new("Изображения", _ImagePatterns);


    #endregion


}
