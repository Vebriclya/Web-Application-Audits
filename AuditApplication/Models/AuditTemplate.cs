using System.ComponentModel.DataAnnotations;

namespace AuditApplication.Models;

public class AuditTemplate
{
    public int Id { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; }
    [Required]
    [MinLength(1, ErrorMessage = "At least one section is required")]
    public List<Section> Sections { get; set; } = new List<Section>();
}