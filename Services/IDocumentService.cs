
using DMS.API.Dto;
using Microsoft.AspNetCore.Http;

namespace DMS.API.Services;

public interface IDocumentService
{
    Task<int> UploadDocumentAsync(IFormFile file, DocumentUploadDto uploadDto);
    Task<int> AddSupportingDocumentAsync(IFormFile file, string primaryDocumentIdentifier, DocumentUploadDto uploadDto);
    Task<DocumentResponseDto> GetDocumentByUniqueIdentifierAsync(string uniqueIdentifier);
    Task<DocumentResponseDto> GetDocumentByIdAsync(int id);
    Task<PrimaryWithSupportingUploadResult> UploadPrimaryWithSupportingDocumentsAsync(
        IFormFile primaryFile,
        DocumentUploadDto primaryUploadDto,
        List<IFormFile> supportingFiles,
        List<DocumentUploadDto> supportingUploadDtos);
}
