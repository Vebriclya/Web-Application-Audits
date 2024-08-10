namespace AuditApplication.Models;

public class QuestionResponse
{
    public int Id { get; set; }
    public int AuditId { get; set; }
    public Audit Audit { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; }
    public RadioResponse RadioAnswer { get; set; }
    public string TextAnswer { get; set; }
    public string AttachmentPath { get; set; }
}

public enum RadioResponse
{
    Excellent,
    Good,
    RequiresImprovement,
    Poor,
    NotAssessed
}