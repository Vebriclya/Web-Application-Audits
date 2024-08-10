using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AuditApplication.Data;
using AuditApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace AuditApplication.Pages.Audits
{
    public class CreateModel : PageModel
    {
        private readonly AuditApplication.Data.AuditContext _context;
        
        public List<AuditTemplate> AvailableTemplates { get;set; }

        public CreateModel(AuditApplication.Data.AuditContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            AvailableTemplates = await _context.AuditTemplates.ToListAsync();
        }
        
        [BindProperty]
        public Audit Audit { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _context.Audits.Add(Audit);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the audit template, please try again.");
                return Page();
            }
            

            
        }
    }
}
