using WPR.Domain.Interfaces;

namespace WPR.Domain.Models.Files;

/// <summary>
/// Фильтр файлов
/// </summary>
public record FileFilter(IEnumerable<string> Extensions, string Description) : IFileFilter;