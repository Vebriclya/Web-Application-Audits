using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AuditApplication.Data;
using AuditApplication.Models;

namespace AuditApplication.Pages.AuditTemplates
{
    public class CreateModel : PageModel
    {
        private readonly AuditApplication.Data.AuditContext _context;

        public CreateModel(AuditApplication.Data.AuditContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public AuditTemplate AuditTemplate { get; set; }
        [BindProperty]
        public List<Section> Sections { get; set; } = new List<Section>();
        [BindProperty]
        public List<Question> Questions { get; set; } = new List<Question>();
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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
    }
}
