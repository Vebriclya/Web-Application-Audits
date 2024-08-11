using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuditApplication.Data;
using AuditApplication.Models;

namespace AuditApplication.Pages.AuditTemplates
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly AuditApplication.Data.AuditContext _context;

        public IndexModel(AuditApplication.Data.AuditContext context)
        {
            _context = context;
        }

        public IList<AuditTemplate> AuditTemplate { get;set; } = default!;

        public async Task OnGetAsync()
        {
            AuditTemplate = await _context.AuditTemplates
                .Include(at => at.Sections)
                .ThenInclude(s => s.Questions)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteTemplateAsync([FromBody] int id)
        {
            var template = await _context.AuditTemplates.FindAsync(id);
            if (template == null)
            {
                return new JsonResult(new { success = false, message = "Template not found." });
            }

            _context.AuditTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}
