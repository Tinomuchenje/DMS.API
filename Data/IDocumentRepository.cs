using DMS.API.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace DMS.API.Data;

public interface IDocumentRepository
{
    Task<Document> GetDocumentByIdAsync(int id);
    Task<Document> GetDocumentByUniqueIdentifierAsync(string uniqueIdentifier);
    Task<IEnumerable<Document>> GetSupportingDocumentsAsync(int primaryDocumentId);
    Task<int> AddDocumentAsync(Document document);
    Task UpdateDocumentAsync(Document document);
    Task<bool> DocumentExistsAsync(string uniqueIdentifier);
    Task<IDbContextTransaction> BeginTransactionAsync();
}
