namespace WPR.Data.Dto;


/// <summary>
/// Данные для передачи файлов по http
/// </summary>
public class FileData
{
    public string Path { get; set; }

    public byte[] Data { get; set; }
}