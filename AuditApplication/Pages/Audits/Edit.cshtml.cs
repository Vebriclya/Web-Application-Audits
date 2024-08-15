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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;


namespace AuditApplication.Pages.Audits
{
    [ValidateAntiForgeryToken]
    public class EditModel : PageModel
    {
        private readonly AuditContext _context;

        public EditModel(AuditContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Audit Audit { get; set; }

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
                Console.WriteLine("Audit is null");
                return NotFound();
            }
            Console.WriteLine($"Audit Id: {Audit.Id}");
            Console.WriteLine($"Attempting to load saved resposes...");

            try
            {
                var questionIds = Audit.Sections
                    .SelectMany(s => s.Questions)
                    .Select(q => q.Id)
                    .ToList();
                var responses = await _context.QuestionResponses
                    .Where(qr => questionIds.Contains(qr.QuestionId))
                    .Select(qr => new { qr.QuestionId, qr.RadioAnswer, qr.TextAnswer })
                    .ToListAsync();
                
                ViewData["Responses"] = responses;
                

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return new JsonResult(new { success = false, error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
        

        public async Task<IActionResult> OnGetSavedResponsesAsync(int id)
        {
            try
            {

                var audit = await _context.Audits
                    .Include(a => a.Sections)
                    .ThenInclude(s => s.Questions)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (audit == null)
                {
                    return new JsonResult(new { success = false, error = "Audit not found" });
                }
                
                var questionIds = audit.Sections
                    .SelectMany(s => s.Questions)
                    .Select(q => q.Id)
                    .ToList();
                
                var responses = await _context.QuestionResponses
                    .Where(qr => questionIds.Contains(qr.QuestionId))
                    .Select(qr => new { qr.QuestionId, qr.RadioAnswer, qr.TextAnswer })
                    .ToListAsync();
                
                return new JsonResult(new { success = true, responses });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return new JsonResult(new { success = false, error = ex.Message, stackTrace = ex.StackTrace });
            }
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
            Console.WriteLine($"Recieved request: QuestionId={request.QuestionId}, AuditId={request.AuditId}, Response={request.Response}");
            try
            {
                var questionExists = await _context.AuditQuestions.AnyAsync(q => q.Id == request.QuestionId);
                if (!questionExists)
                {
                    return BadRequest(new { message = "Invalid QuestionId" });
                }

                var response = await _context.QuestionResponses
                    .FirstOrDefaultAsync(qr => qr.QuestionId == request.QuestionId);

                if (response == null)
                {
                    response = new QuestionResponse 
                    { 
                        QuestionId = request.QuestionId, 
                        RadioAnswer = (RadioResponse)request.Response,
                        TextAnswer = request.TextAnswer
                    };
                    _context.QuestionResponses.Add(response);
                }
                else
                {
                    response.RadioAnswer = (RadioResponse)request.Response;
                    if (request.TextAnswer != null)
                    {
                        response.TextAnswer = request.TextAnswer;
                    }
                }

                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "An error occurred", 
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        public class UpdateQuestionResponseRequest
        {
            public int QuestionId { get; set; }
            public int AuditId { get; set; }
            public int Response { get; set; }
            public string? TextAnswer { get; set;}
        }

        public async Task<IActionResult> OnPostUpdateQuestionCommentAsync(
            [FromBody] UpdateQuestionCommentRequest request)
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
            sb.Append($"<div class='text-center mb-4'><h4 class='section-name'>{section.Name}</h4></div>");
            sb.Append("<div class='questions'>");

            foreach (var question in section.Questions)
            {
                sb.Append($"<div class='question mb-3 pb-3 border-bottom' data-question-id='{question.Id}'>");
                sb.Append($"<div class='d-flex justify-content-between align-items-center'>");
                sb.Append($"<h5 class='question-text mb-0'>{question.Text}</h5>");
                sb.Append(
                    "<div><a href='#' class='edit-question me-2'>Edit</a><a href='#' class='delete-question'>Delete</a></div>");
                sb.Append("</div>");

                // Radio buttons
                sb.Append("<div class='response-options'>");
                foreach (RadioResponse option in Enum.GetValues(typeof(RadioResponse)))
                {
                    string displayText = option.ToString();
                    if (displayText == "RequiresImprovement") displayText = "Requires Improvement";
                    if (displayText == "NotAssessed") displayText = "Not Assessed";

                    sb.Append($"<div class='form-check form-check-inline'>");
                    sb.Append(
                        $"<input class='form-check-input' type='radio' name='question-{question.Id}' id='question-{question.Id}-{option}' value='{option}' />");
                    sb.Append(
                        $"<label class='form-check-label' for='question-{question.Id}-{option}'>{displayText}</label>");
                    sb.Append("</div>");
                }

                sb.Append("</div>");

                sb.Append("<div class='mt-2 d-flex justify-content-end'>");
                sb.Append("<button class='btn btn-secondary btn-sm attachments-btn me-2'>Attachments</button>");
                sb.Append("<button class='btn btn-secondary btn-sm comments-btn'>Comments</button>");
                sb.Append("</div>");

                sb.Append("<div class='comments-accordion mt-2' style='display: none;'>");
                sb.Append("<div class='card'><div class='card-body'>");
                sb.Append("<textarea class='form-control comment-text' rows='3'></textarea>");
                sb.Append("<button class='btn btn-primary btn-sm mt-2 save-comment'>Save</button>");
                sb.Append("</div></div></div>");

                sb.Append("</div>");
            }

            sb.Append("</div></div>");
            return sb.ToString();
        }
    }
}