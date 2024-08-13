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
using System.Text;


namespace AuditApplication.Pages.Audits
{
    public class EditModel : PageModel
    {
        private readonly AuditContext _context;

        public EditModel(AuditContext context)
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

            Audit = await _context.Audits
                .Include(a => a.Sections)
                .ThenInclude(s => s.Questions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Audit == null)
            {
                return NotFound();
            }

            return Page();
        }
        
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

        public async Task<IActionResult> OnPostUpdateSectionAsync([FromBody] UpdateSectionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var section = await _context.AuditSections.FindAsync(request.Id);
            if (section == null)
            {
                return NotFound();
            }

            section.Name = request.Name;

            try
            {
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuditSectionExists(section.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        
        private bool AuditSectionExists(int id)
        {
            return _context.AuditSections.Any(e => e.Id == id);
        }
        
        public class UpdateSectionRequest
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public async Task<IActionResult> OnPostUpdateQuestionAsync([FromBody] UpdateQuestionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = await _context.AuditQuestions.FindAsync(request.Id);
            if (question == null)
            {
                return NotFound();
            }

            question.Text = request.Text;

            try
            {
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuditQuestionExists(question.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        
        private bool AuditQuestionExists(int id)
        {
            return _context.AuditQuestions.Any(e => e.Id == id);
        }
        
        public class UpdateQuestionRequest
        {
            public int Id { get; set; }
            public string Text { get; set; }
        }

        public async Task<IActionResult> OnPostUpdateQuestionResponseAsync([FromBody] UpdateQuestionResponseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _context.QuestionResponses
                .FirstOrDefaultAsync(qr => qr.QuestionId == request.QuestionId && qr.AuditId == request.AuditId);

            if (response == null)
            {
                response = new QuestionResponse { QuestionId = request.QuestionId, AuditId = request.AuditId };
                _context.QuestionResponses.Add(response);
            }

            response.RadioAnswer = request.Response;

            try
            {
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                return new JsonResult(new { success = false, message = "An error occurred while saving the response." });
            }
        }

        public class UpdateQuestionResponseRequest
        {
            public int QuestionId { get; set; }
            public int AuditId { get; set; }
            public RadioResponse Response { get; set; }
        }
        
        public async Task<IActionResult> OnPostUpdateQuestionCommentAsync([FromBody] UpdateQuestionCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _context.QuestionResponses.FindAsync(request.QuestionId);
            if (response == null)
            {
                response = new QuestionResponse { QuestionId = request.QuestionId };
                _context.QuestionResponses.Add(response);
            }

            response.TextAnswer = request.Comment;

            try
            {
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                return new JsonResult(new { success = false, message = "An error occurred while saving the comment." });
            }
        }

        public class UpdateQuestionCommentRequest
        {
            public int QuestionId { get; set; }
            public string Comment { get; set; }
        }
        
        private bool QuestionResponseExists(int id)
        {
            return _context.QuestionResponses.Any(e => e.Id == id);
        }
        
        public async Task<IActionResult> OnGetSectionDetailsAsync(int sectionId)
        {
            var section = await _context.AuditSections
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == sectionId);

            if (section == null)
            {
                return NotFound();
            }

            var sectionDetailsHtml = GenerateSectionDetailsHtml(section);
            return new JsonResult(new { sectionDetailsHtml });
        }
        
        private string GenerateSectionDetailsHtml(AuditSection section)
        {
            var sb = new StringBuilder();
            sb.Append($"<div class='section' data-section-id='{section.Id}' data-audit-id='{section.AuditId}'>");
            sb.Append($"<h4 class='section-name'>{section.Name}</h4>");
            sb.Append("<div class='questions'>");

            foreach (var question in section.Questions)
            {
                sb.Append($"<div class='question' data-question-id='{question.Id}'>");
                sb.Append($"<p>{question.Text}</p>");
                sb.Append("<div class='response-options'>");
                foreach (RadioResponse option in Enum.GetValues(typeof(RadioResponse)))
                {
                    sb.Append($"<label><input type='radio' name='question-{question.Id}' value='{option}' /> {option}</label>");
                }
                sb.Append("</div>");
                sb.Append("<textarea class='form-control mt-2' rows='3' placeholder='Additional comments...'></textarea>");
                sb.Append("</div>");
            }

            sb.Append("</div></div>");
            return sb.ToString();
        }

        }
    }
