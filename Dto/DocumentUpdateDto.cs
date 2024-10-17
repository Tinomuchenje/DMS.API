using DMS.API.Models;

public class DocumentUpdateDto
{
    public string? FileName { get; set; }
    public DocumentType? Type { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
