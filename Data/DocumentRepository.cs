using DMS.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DMS.API.Data;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Document> GetDocumentByIdAsync(int id)
    {
        return await _context.Documents
            .Include(d => d.SupportingDocuments)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Document> GetDocumentByUniqueIdentifierAsync(string uniqueIdentifier)
    {
        return await _context.Documents
            .Include(d => d.SupportingDocuments)
            .FirstOrDefaultAsync(d => d.UniqueIdentifier == uniqueIdentifier);
    }

    public async Task<IEnumerable<Document>> GetSupportingDocumentsAsync(int primaryDocumentId)
    {
        return await _context.Documents
            .Where(d => d.PrimaryDocumentId == primaryDocumentId)
            .ToListAsync();
    }

    public async Task<int> AddDocumentAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document.Id;
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        _context.Entry(document).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DocumentExistsAsync(string uniqueIdentifier)
    {
        return await _context.Documents.AnyAsync(d => d.UniqueIdentifier == uniqueIdentifier);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}