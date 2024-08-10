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
        private readonly AuditApplication.Data.AuditContext _context;

        public EditModel(AuditApplication.Data.AuditContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AuditTemplate AuditTemplate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var audittemplate =  await _context.AuditTemplates.FirstOrDefaultAsync(m => m.Id == id);
            if (audittemplate == null)
            {
                return NotFound();
            }
            AuditTemplate = audittemplate;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
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
    }
}
