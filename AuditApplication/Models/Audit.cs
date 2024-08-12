namespace AuditApplication.Models;

public class Audit
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public List<AuditSection> Sections { get; set; } = new List<AuditSection>();
    public List<QuestionResponse> QuestionResponses { get; set; } = new List<QuestionResponse>();
}