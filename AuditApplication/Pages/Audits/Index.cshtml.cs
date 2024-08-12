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

        public IList<Audit> Audit { get;set; } = default!;

        public async Task OnGetAsync()
        {

        }
    }
}
