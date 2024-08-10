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
    public class CreateModel : PageModel
    {
        private readonly AuditContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(AuditContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        [BindProperty]
        public AuditTemplate AuditTemplate { get; set; }

        public async Task<IActionResult> OnPostCreateTemplateAsync([FromBody] AuditTemplate template)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _context.AuditTemplates.Add(template);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = template.Id });
        }

        public async Task<IActionResult> OnPostAddSectionAsync([FromBody] Section section)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _context.Sections.Add(section);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = section.Id });
        }

        public async Task<IActionResult> OnPostAddQuestionAsync([FromBody] Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = question.Id });
        }

        // public async Task<IActionResult> OnPostUpdateOrderAsync([FromBody] List<OrderItem> items)
        // {
        //     
        // }
        public IActionResult OnGet()
        {
            AuditTemplate = new AuditTemplate();
            return Page();
        }
        
        [BindProperty]
        public List<Section> Sections { get; set; } = new List<Section>();
        [BindProperty]
        public List<Question> Questions { get; set; } = new List<Question>();
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation($"Attempting to create template with {Sections?.Count ?? 0} sections and {Questions?.Count ?? 0} questions");
            if (Sections == null || !Sections.Any())
            {
                ModelState.AddModelError("", "At least one section is required");
                return Page();
            }
            
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogError($"Validation error: {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            try
            {
                _context.AuditTemplates.Add(AuditTemplate);
                await _context.SaveChangesAsync();

                foreach (var section in Sections)
                {
                    section.AuditTemplateId = AuditTemplate.Id;
                    _context.Sections.Add(section);
                }
                await _context.SaveChangesAsync();

                foreach (var question in Questions)
                {
                    _context.Questions.Add(question);
                }
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving audit template: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while saving the audit template, please try again.");
                return Page();
            }
        }
    }
}
