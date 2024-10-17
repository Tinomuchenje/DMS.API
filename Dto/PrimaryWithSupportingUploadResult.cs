namespace DMS.API.Dto;

public class PrimaryWithSupportingUploadResult
{
    public int PrimaryDocumentId { get; set; }
    public List<int>? SupportingDocumentIds { get; set; }
}
