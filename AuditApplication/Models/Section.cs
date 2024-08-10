namespace AuditApplication.Models;

public class Section
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public int AuditTemplateId { get; set; }
    public AuditTemplate AuditTemplate { get; set; }
    public List<Question> Questions { get; set; } = new List<Question>();
}