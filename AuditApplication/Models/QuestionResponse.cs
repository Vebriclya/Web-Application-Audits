namespace AuditApplication.Models;

public class QuestionResponse
{
    public int Id { get; set; }
    public int AuditId { get; set; }
    public int QuestionId { get; set; }
    public RadioResponse RadioAnswer { get; set; }
    public string? TextAnswer { get; set; }
    public string? AttachmentPath { get; set; }
}

public enum RadioResponse
{
    Excellent = 100,
    Good = 75,
    RequiresImprovement = 50,
    Poor = 25,
    NotAssessed = 0
}