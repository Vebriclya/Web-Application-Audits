using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AuditApplication.Data;
using AuditApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace AuditApplication.Pages.AuditTemplates
{
    [IgnoreAntiforgeryToken]
    public class CreateModel : PageModel
    {
        private readonly AuditContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(AuditContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty] public AuditTemplate AuditTemplate { get; set; }

        public async Task<IActionResult> OnPostCreateTemplateAsync([FromBody] CreateTemplateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Template name is required.");
            }
            
            var template = new AuditTemplate { Name = request.Name };
            _context.AuditTemplates.Add(template);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = template.Id });
        }

        public class CreateTemplateRequest
        {
            public string Name { get; set; }
        }
        
        public async Task<IActionResult> OnPostUpdateTemplateAsync([FromBody] UpdateTemplateRequest request)
        {
            var template = await _context.AuditTemplates.FindAsync(request.Id);
            if (template == null)
            {
                return NotFound();
            }
            template.Name = request.Name;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = template.Id });
        }

        public class UpdateTemplateRequest
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public async Task<IActionResult> OnPostAddSectionAsync([FromBody] AddSectionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Section name is required.");
            }
            var template = await _context.AuditTemplates.FindAsync(request.AuditTemplateId);
            if (template == null)
            {
                return BadRequest(new { message = "Invalid template id." });
            }
            
            var section = new Section
            {
                Name = request.Name, 
                AuditTemplateId = request.AuditTemplateId
            };
            
            _context.Sections.Add(section);
            await _context.SaveChangesAsync();
            
            return new JsonResult(new { success = true, id = section.Id });
        }

        public class AddSectionRequest
        {
            public string Name { get; set; }
            public int AuditTemplateId { get; set; }
        }
        
        public async Task<IActionResult> OnPostUpdateSectionAsync([FromBody] UpdateItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return new JsonResult(new { success = false, message = "Section name cannot be empty." });
            }

            var section = await _context.Sections.FindAsync(request.Id);
            if (section == null)
            {
                return NotFound();
            }
            section.Name = request.Text.Trim();
            try
            {
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating section");
                return new JsonResult(new { success = false, message = "An error occurred while updating the section." });
            }
        }
        
        public async Task<IActionResult> OnPostDeleteSectionAsync([FromBody] int id)
        {
            try
            {
                var section = await _context.Sections.FindAsync(id);
                if (section == null)
                {
                    return new JsonResult(new { success = false, message = "Section not found." });
                }

                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();
        
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting section with ID {id}");
                return new JsonResult(new { success = false, message = "An error occurred while deleting the section." });
            }
        }

        public async Task<IActionResult> OnPostAddQuestionAsync([FromBody] AddQuestionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return new JsonResult(new { success = false, message = "Question text cannot be empty." });
            }

            var question = new Question
            {
                Text = request.Text,
                SectionId = request.SectionId,
                Order = _context.Questions.Count(q => q.SectionId == request.SectionId) + 1
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = question.Id });
        }

        public class AddQuestionRequest
        {
            public string Text { get; set; }
            public int SectionId { get; set; }
        }
        
        public async Task<IActionResult> OnPostUpdateQuestionAsync([FromBody] UpdateItemRequest request)
        {
            var question = await _context.Questions.FindAsync(request.Id);
            if (question == null)
            {
                return NotFound();
            }
            question.Text = request.Text;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
        
        public async Task<IActionResult> OnPostDeleteQuestionAsync([FromBody]int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return new JsonResult(new { success = false, message = "Question not found." });
            }
            
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
        
        public class UpdateItemRequest
        {
            public int Id { get; set; }
            public string Text { get; set; }
        }

        public JsonResult OnGetExistingTemplates()
        {
            var templates = _context.AuditTemplates
                .Select(t => new { id = t.Id, name = t.Name })
                .ToList();
            return new JsonResult(templates);
        }

        public async Task<IActionResult> OnPostDuplicateTemplateAsync([FromBody] int id)
        {
            var existingTemplate = await _context.AuditTemplates
                .Include(t => t.Sections)
                .ThenInclude(s => s.Questions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTemplate == null)
            {
                return NotFound();
            }

            var newTemplate = new AuditTemplate
            {
                Name = $"Copy of {existingTemplate.Name}",
                Sections = existingTemplate.Sections.Select(s => new Section
                {
                    Name = s.Name,
                    Order = s.Order,
                    Questions = s.Questions.Select(q => new Question
                    {
                        Text = q.Text,
                        Order = q.Order
                    }).ToList()
                }).ToList()
            };

            _context.AuditTemplates.Add(newTemplate);
            await _context.SaveChangesAsync();

            var results = new
            {
                success = true,
                id = newTemplate.Id,
                name = newTemplate.Name,
                sections = newTemplate.Sections.Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    questions = s.Questions.Select(q => new
                    {
                        id = q.Id,
                        text = q.Text
                    })
                })
            };

            return new JsonResult(results);
        }

    }
}
