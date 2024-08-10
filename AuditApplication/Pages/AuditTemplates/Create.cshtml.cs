using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AuditApplication.Data;
using AuditApplication.Models;
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

        public async Task<IActionResult> OnPostAddSectionAsync([FromBody] Section section)
        {
            if (string.IsNullOrWhiteSpace(section.Name))
            {
                return new JsonResult(new { success = false, message = "Section name cannot be empty." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = section.Id });
        }
        
        public async Task<IActionResult> OnPostUpdateSectionAsync([FromBody] UpdateItemRequest request)
        {
            var section = await _context.Sections.FindAsync(request.Id);
            if (section == null)
            {
                return NotFound();
            }
            section.Name = request.Text;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
        
        public async Task<IActionResult> OnDeleteSectionAsync(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section != null)
            {
                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAddQuestionAsync([FromBody] Question question)
        {
            if (string.IsNullOrWhiteSpace(question.Text))
            {
                return new JsonResult(new { success = false, message = "Question text cannot be empty." });
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = question.Id });
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
        
        public async Task<IActionResult> OnDeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }

            return NotFound();
        }
        
        public class UpdateItemRequest
        {
            public int Id { get; set; }
            public string Text { get; set; }
        }
        
    }
}
