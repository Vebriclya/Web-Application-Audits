using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuditApplication.Data;
using AuditApplication.Models;

namespace AuditApplication.Pages.Audits
{
    public class DeleteModel : PageModel
    {
        private readonly AuditApplication.Data.AuditContext _context;

        public DeleteModel(AuditApplication.Data.AuditContext context)
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

            var audit = await _context.Audits.FirstOrDefaultAsync(m => m.Id == id);

            if (audit == null)
            {
                return NotFound();
            }
            else
            {
                Audit = audit;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var audit = await _context.Audits.FindAsync(id);
            if (audit != null)
            {
                Audit = audit;
                _context.Audits.Remove(Audit);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
