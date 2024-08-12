namespace AuditApplication.Models;

public class AuditSection
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public int AuditId { get; set; }
    public Audit Audit { get; set; }
    public List<AuditQuestion> Questions { get; set; } = new List<AuditQuestion>();
}