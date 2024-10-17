using DMS.API.Data;
using DMS.API.Models;
using DMS.API.Dto;
using DMS.API.Helpers;

namespace DMS.API.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(IDocumentRepository documentRepository, IConfiguration configuration, ILogger<DocumentService> logger)
    {
        _documentRepository = documentRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<int> UploadDocumentAsync(IFormFile file, DocumentUploadDto uploadDto)
    {
        try
        {
            string filePath = await FileHelper.SaveFileAsync(file, _configuration["FileStorage:Path"] ?? "");

            var document = new Document
            {
                FileName = file.FileName,
                FilePath = filePath,
                Type = uploadDto.Type,
                UniqueIdentifier = uploadDto.UniqueIdentifier,
                Metadata = uploadDto.Metadata
            };

            return await _documentRepository.AddDocumentAsync(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document");
            throw;
        }
    }

    public async Task<int> AddSupportingDocumentAsync(IFormFile file, string primaryDocumentIdentifier, DocumentUploadDto uploadDto)
    {
        try
        {
            var primaryDocument = await _documentRepository.GetDocumentByUniqueIdentifierAsync(primaryDocumentIdentifier);
            if (primaryDocument == null)
            {
                throw new ArgumentException("Primary document not found");
            }

            string filePath = await FileHelper.SaveFileAsync(file, _configuration["FileStorage:Path"] ?? "");

            var supportingDocument = new Document
            {
                FileName = file.FileName,
                FilePath = filePath,
                Type = DocumentType.Supporting,
                UniqueIdentifier = uploadDto.UniqueIdentifier,
                Metadata = uploadDto.Metadata,
                PrimaryDocumentId = primaryDocument.Id
            };

            return await _documentRepository.AddDocumentAsync(supportingDocument);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding supporting document");
            throw;
        }
    }

    public async Task<DocumentResponseDto> GetDocumentByUniqueIdentifierAsync(string uniqueIdentifier)
    {
        var document = await _documentRepository.GetDocumentByUniqueIdentifierAsync(uniqueIdentifier);
        return document != null ? MapDocumentToResponseDto(document) : throw new InvalidOperationException("Document not found.");
    }

    public async Task<DocumentResponseDto> GetDocumentByIdAsync(int id)
    {
        var document = await _documentRepository.GetDocumentByIdAsync(id);
        return document != null ? MapDocumentToResponseDto(document) : throw new InvalidOperationException("Document not found.");
    }

    public async Task<PrimaryWithSupportingUploadResult> UploadPrimaryWithSupportingDocumentsAsync(
        IFormFile primaryFile,
        DocumentUploadDto primaryUploadDto,
        List<IFormFile> supportingFiles,
        List<DocumentUploadDto> supportingUploadDtos)
    {
        var result = new PrimaryWithSupportingUploadResult();

        using var transaction = await _documentRepository.BeginTransactionAsync();
        try
        {
            // Upload primary document
            result.PrimaryDocumentId = await UploadDocumentAsync(primaryFile, primaryUploadDto);

            // Upload supporting documents
            result.SupportingDocumentIds = new List<int>();
            foreach (var (file, dto) in supportingFiles.Zip(supportingUploadDtos, (f, d) => (f, d)))
            {
                dto.Type = DocumentType.Supporting;
                int supportingDocId = await AddSupportingDocumentAsync(file, primaryUploadDto.UniqueIdentifier, dto);
                result.SupportingDocumentIds.Add(supportingDocId);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return result;
    }

    private DocumentResponseDto MapDocumentToResponseDto(Document document)
    {
        return new DocumentResponseDto
        {
            Id = document.Id,
            FileName = document.FileName,
            Type = document.Type,
            UniqueIdentifier = document.UniqueIdentifier,
            Metadata = document.Metadata,
            SupportingDocuments = document.SupportingDocuments?.Select(d => new DocumentResponseDto
            {
                Id = d.Id,
                FileName = d.FileName,
                Type = d.Type,
                UniqueIdentifier = d.UniqueIdentifier,
                Metadata = d.Metadata
            }).ToList()
        };
    }
}