namespace AuditApplication.Models;

public class AuditQuestion
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int Order { get; set; }
    public int AuditSectionId { get; set; }
}