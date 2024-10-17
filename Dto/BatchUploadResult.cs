namespace DMS.API.Dto;

public class BatchUploadResult
{
    public string FileName { get; set; }
    public int? DocumentId { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}