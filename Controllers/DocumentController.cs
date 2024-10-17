using Microsoft.AspNetCore.Mvc;
using DMS.API.Services;
using DMS.API.Dto;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace DMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentController> _logger;

    public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocument([FromForm] IFormFile file, [FromForm] DocumentUploadDto uploadDto)
    {
        try
        {
            int documentId = await _documentService.UploadDocumentAsync(file, uploadDto);
            return Ok(new { DocumentId = documentId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document");
            return StatusCode(500, "An error occurred while uploading the document");
        }
    }

    [HttpPost("upload-batch")]
    public async Task<IActionResult> UploadBatchDocuments([FromForm] List<IFormFile> files, [FromForm] List<DocumentUploadDto> uploadDtos)
    {
        if (files.Count != uploadDtos.Count)
        {
            return BadRequest("The number of files and upload DTOs must match.");
        }

        try
        {
            var results = new ConcurrentBag<BatchUploadResult>();
            var tasks = new List<Task>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var dto = uploadDtos[i];
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        int documentId = await _documentService.UploadDocumentAsync(file, dto);
                        results.Add(new BatchUploadResult { FileName = file.FileName, DocumentId = documentId, Success = true });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error uploading document: {file.FileName}");
                        results.Add(new BatchUploadResult { FileName = file.FileName, Success = false, ErrorMessage = ex.Message });
                    }
                }));
            }

            await Task.WhenAll(tasks);

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch upload");
            return StatusCode(500, "An error occurred during batch upload");
        }
    }

    [HttpPost("upload-primary-with-supporting")]
    public async Task<IActionResult> UploadPrimaryWithSupportingDocuments([FromForm] IFormFile primaryFile, [FromForm] DocumentUploadDto primaryUploadDto, [FromForm] List<IFormFile> supportingFiles, [FromForm] List<DocumentUploadDto> supportingUploadDtos)
    {
        try
        {
            var result = await _documentService.UploadPrimaryWithSupportingDocumentsAsync(primaryFile, primaryUploadDto, supportingFiles, supportingUploadDtos);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading primary document with supporting documents");
            return StatusCode(500, "An error occurred during the upload process");
        }
    }

    [HttpPost("upload-supporting/{primaryDocumentIdentifier}")]
    public async Task<IActionResult> UploadSupportingDocument(string primaryDocumentIdentifier, [FromForm] IFormFile file, [FromForm] DocumentUploadDto uploadDto)
    {
        try
        {
            int documentId = await _documentService.AddSupportingDocumentAsync(file, primaryDocumentIdentifier, uploadDto);
            return Ok(new { DocumentId = documentId });
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading supporting document.");
            return StatusCode(500, "An error occurred while uploading the supporting document");
        }
    }

    [HttpGet("{uniqueIdentifier}")]
    public async Task<IActionResult> GetDocumentByUniqueIdentifier(string uniqueIdentifier)
    {
        var document = await _documentService.GetDocumentByUniqueIdentifierAsync(uniqueIdentifier);
        if (document == null)
        {
            return NotFound();
        }
        return Ok(document);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetDocumentById(int id)
    {
        var document = await _documentService.GetDocumentByIdAsync(id);
        if (document == null)
        {
            return NotFound();
        }
        return Ok(document);
    }

}