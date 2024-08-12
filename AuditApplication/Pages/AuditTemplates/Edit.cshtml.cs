using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuditApplication.Data;
using AuditApplication.Models;

namespace AuditApplication.Pages.AuditTemplates
{
    public class EditModel : PageModel
    {
        private readonly AuditContext _context;
        private readonly ILogger<EditModel> _logger;
        
        public EditModel(AuditContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        

        [BindProperty]
        public AuditTemplate AuditTemplate { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            _logger.LogInformation($"Received id: { id }");
            if (id == null)
            {
                return NotFound();
            }

            AuditTemplate = await _context.AuditTemplates
                .Include(at => at.Sections)
                .ThenInclude(s => s.Questions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (AuditTemplate == null)
            {
                return NotFound();
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(AuditTemplate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuditTemplateExists(AuditTemplate.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AuditTemplateExists(int id)
        {
            return _context.AuditTemplates.Any(e => e.Id == id);
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
    }
}
