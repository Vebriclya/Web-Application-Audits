namespace AuditApplication.Models;

public class AuditTemplate
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Section> Sections { get; set; } = new List<Section>();
}