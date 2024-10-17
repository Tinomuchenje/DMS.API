using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.API.Models;

public class Document
{
    [Key]
    public int Id { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public DocumentType Type { get; set; }
    public required string UniqueIdentifier { get; set; }
    [Column(TypeName = "jsonb")]
    public required Dictionary<string, object> Metadata { get; set; }
    public int? PrimaryDocumentId { get; set; }
    public Document? PrimaryDocument { get; set; }
    public ICollection<Document>? SupportingDocuments { get; set; }
}
