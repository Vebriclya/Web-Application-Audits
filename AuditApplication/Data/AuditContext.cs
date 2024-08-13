using Microsoft.EntityFrameworkCore;
using AuditApplication.Models;
namespace AuditApplication.Data;

public class AuditContext : DbContext
{
    public AuditContext(DbContextOptions<AuditContext> options) : base(options)
    {
    }

    public DbSet<AuditTemplate> AuditTemplates { get; set; }
    public DbSet<Audit> Audits { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionResponse> QuestionResponses { get; set; }
    public DbSet<AuditSection> AuditSections { get; set; }
    public DbSet<AuditQuestion> AuditQuestions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Additional configurations can be added here, see: https://docs.microsoft.com/en-us/ef/core/modeling/
    }
}