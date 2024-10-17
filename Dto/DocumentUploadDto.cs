using DMS.API.Models;

namespace DMS.API.Dto;

public class DocumentUploadDto
{
    public DocumentType Type { get; set; }
    public required string UniqueIdentifier { get; set; }
    public required Dictionary<string, object> Metadata { get; set; }
}
