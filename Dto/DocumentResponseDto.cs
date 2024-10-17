using DMS.API.Models;

namespace DMS.API.Dto;

public class DocumentResponseDto
{
    public int Id { get; set; }
    public string? FileName { get; set; }
    public DocumentType Type { get; set; }
    public required string UniqueIdentifier { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public List<DocumentResponseDto>? SupportingDocuments { get; set; }
}