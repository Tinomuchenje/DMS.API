using Microsoft.EntityFrameworkCore;
using DMS.API.Models;

namespace DMS.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>()
            .HasMany(d => d.SupportingDocuments)
            .WithOne(d => d.PrimaryDocument)
            .HasForeignKey(d => d.PrimaryDocumentId);
    }
}