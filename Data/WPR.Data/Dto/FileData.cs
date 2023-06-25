namespace WPR.Data.Dto;


/// <summary>
/// Данные для передачи файлов по http
/// </summary>
/// <param name="Path">Путь к файлу</param>
/// <param name="Data">Двоичные данные файла</param>
public record FileData(string Path, byte[] Data);