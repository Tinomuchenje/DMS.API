using DMS.API.Models;

public class DocumentSearchDto
{
    public string? Keyword { get; set; }
    public DocumentType? Type { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    // Add other search criteria as needed
}
