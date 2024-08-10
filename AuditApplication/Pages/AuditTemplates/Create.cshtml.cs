using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AuditApplication.Data;
using AuditApplication.Models;
using Microsoft.Extensions.Logging;

namespace AuditApplication.Pages.AuditTemplates
{
    public class CreateModel : PageModel
    {
        private readonly AuditContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(AuditContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty] public AuditTemplate AuditTemplate { get; set; }

        public async Task<IActionResult> OnPostCreateTemplateAsync([FromBody] AuditTemplate template)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AuditTemplates.Add(template);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = template.Id });
        }

        public async Task<IActionResult> OnPostAddSectionAsync([FromBody] Section section)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = section.Id });
        }

        public async Task<IActionResult> OnPostAddQuestionAsync([FromBody] Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = question.Id });
        }

        public async Task<IActionResult> OnPostUpdateOrderAsync([FromBody] List<OrderItem> items)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in items)
                {
                    if (item.Type == "section")
                    {
                        var section = await _context.Sections.FindAsync(item.Id);
                        if (section != null)
                        {
                            section.Order = item.Order;
                            _context.Sections.Update(section);
                        }
                    }
                    else if (item.Type == "question")
                    {
                        var question = await _context.Questions.FindAsync(item.Id);
                        if (question != null)
                        {
                            question.Order = item.Order;
                            _context.Questions.Update(question);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating order");
                return new JsonResult(new { success = false, message = "An error occurred while updating the order." });
            }
        }

        public async Task<IActionResult> OnDeleteSectionAsync(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section != null)
            {
                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }

            return NotFound();
        }

        public async Task<IActionResult> OnDeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }

            return NotFound();
        }

        public class OrderItem
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public int Order { get; set; }
        }
    }
}
