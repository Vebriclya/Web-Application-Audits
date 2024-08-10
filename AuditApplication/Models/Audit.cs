namespace AuditApplication.Models;

public class Audit
{
    public int Id { get; set; }
    public int TemplateId { get; set; }
    public AuditTemplate Template { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public List<QuestionResponse> QuestionResponses { get; set; } = new List<QuestionResponse>();
}