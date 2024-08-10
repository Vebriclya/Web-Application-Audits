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
    public class DetailsModel : PageModel
    {
        private readonly AuditApplication.Data.AuditContext _context;

        public DetailsModel(AuditApplication.Data.AuditContext context)
        {
            _context = context;
        }

        public AuditTemplate AuditTemplate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var audittemplate = await _context.AuditTemplates.FirstOrDefaultAsync(m => m.Id == id);
            if (audittemplate == null)
            {
                return NotFound();
            }
            else
            {
                AuditTemplate = audittemplate;
            }
            return Page();
        }
    }
}
