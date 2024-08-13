namespace AuditApplication.Models;

public class QuestionResponse
{
    public int Id { get; set; }
    public int AuditId { get; set; }
    public Audit Audit { get; set; }
    public int QuestionId { get; set; }
    public AuditQuestion Question { get; set; }
    public RadioResponse RadioAnswer { get; set; }
    public string? TextAnswer { get; set; }
    public string? AttachmentPath { get; set; }
}

public enum RadioResponse
{
    Excellent = 0,
    Good = 75,
    RequiresImprovement = 50,
    Poor = 25,
    NotAssessed = 0
}