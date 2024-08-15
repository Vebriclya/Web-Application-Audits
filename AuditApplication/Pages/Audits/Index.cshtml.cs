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
    [ValidateAntiForgeryToken]
    public class IndexModel : PageModel
    {
        private readonly AuditApplication.Data.AuditContext _context;

        public IndexModel(AuditApplication.Data.AuditContext context)
        {
            _context = context;
        }

        public IList<Audit> Audits { get;set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? FilterOption { get; set; }

        public async Task OnGetAsync()
        {
            // Little iffy on this include, remove it if it smells bad
            IQueryable<Audit> auditsQuery = _context.Audits.Include(a => a.Sections);

            if (FilterOption == "Completed")
            {
                auditsQuery = auditsQuery.Where(a => a.CompletionDate != null);
            }
            else if (FilterOption == "Uncompleted")
            {
                auditsQuery = auditsQuery.Where(a => a.CompletionDate == null);
            }

            Audits = await auditsQuery.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAuditAsync(int id)
        {
            try
            {
                var audit = await _context.Audits
                    .Include(a => a.Sections)
                    .ThenInclude(s => s.Questions)
                    .Include(a => a.QuestionResponses)
                    .FirstOrDefaultAsync(a => a.Id == id);
                
                if (audit!= null)
                {
                    _context.AuditQuestions.RemoveRange(audit.Sections.SelectMany(s => s.Questions));
                    _context.AuditSections.RemoveRange(audit.Sections);
                    _context.QuestionResponses.RemoveRange(audit.QuestionResponses);
                    _context.Audits.Remove(audit);
                    await _context.SaveChangesAsync();
                }
                return new JsonResult(new { success = true, message = "Audit deleted successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
