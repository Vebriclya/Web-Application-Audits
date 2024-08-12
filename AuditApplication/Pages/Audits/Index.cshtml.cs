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
            IQueryable<Audit> auditsQuery = _context.Audits;

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
    }
}
