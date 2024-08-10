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

namespace AuditApplication.Pages.Audits
{
    public class EditModel : PageModel
    {
        private readonly AuditApplication.Data.AuditContext _context;

        public EditModel(AuditApplication.Data.AuditContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Audit Audit { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var audit =  await _context.Audits.FirstOrDefaultAsync(m => m.Id == id);
            if (audit == null)
            {
                return NotFound();
            }
            Audit = audit;
           ViewData["TemplateId"] = new SelectList(_context.AuditTemplates, "Id", "Id");
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

            _context.Attach(Audit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuditExists(Audit.Id))
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

        private bool AuditExists(int id)
        {
            return _context.Audits.Any(e => e.Id == id);
        }
    }
}
