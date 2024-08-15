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
using System.Text;
using Microsoft.Extensions.Primitives;

namespace AuditApplication.Pages.Audits
{
    [ValidateAntiForgeryToken]
    public class CreateModel : PageModel
    {
        private readonly AuditContext _context;

        public CreateModel(AuditContext context)
        {
            _context = context;
        }
        
        public List<AuditTemplate> AvailableTemplates { get;set; }

        public async Task OnGetAsync()
        {
            AvailableTemplates = await _context.AuditTemplates.ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateAuditFromTemplateAsync([FromBody] CreateAuditRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
    
            var template = await _context.AuditTemplates
                .Include(t => t.Sections)
                .ThenInclude(s => s.Questions)
                .FirstOrDefaultAsync(t => t.Id == request.TemplateId);

            if (template == null)
            {
                return new JsonResult(new { success = false, message = "Template not found" });
            }

            var audit = new Audit
            {
                AuditName = $"{template.Name} - Audit",
                StartDate = DateTime.Now
            };

            foreach (var templateSection in template.Sections)
            {
                var auditSection = new AuditSection
                {
                    Name = templateSection.Name,
                    Order = templateSection.Order,
                    AuditId = audit.Id
                };

                foreach (var templateQuestion in templateSection.Questions)
                {
                    var auditQuestion = new AuditQuestion
                    {
                        Text = templateQuestion.Text,
                        Order = templateQuestion.Order
                    };
                    auditSection.Questions.Add(auditQuestion);
                }
                audit.Sections.Add(auditSection);
            }

            _context.Audits.Add(audit);
            await _context.SaveChangesAsync();

            var sectionListHtml = GenerateSectionListHtml(audit.Sections);
            var sectionDetailsHtml = GenerateSectionDetailsHtml(audit.Sections.FirstOrDefault());

            return new JsonResult(new { 
                success = true, 
                auditName = audit.AuditName,
                sectionListHtml = sectionListHtml, 
                sectionDetailsHtml = sectionDetailsHtml 
            });
        }

        public class CreateAuditRequest
        {
            public int TemplateId { get; set; }
        }
        
        private string GenerateSectionListHtml(IEnumerable<AuditSection> sections)
        {
            var sb = new StringBuilder();
            foreach (var section in sections)
            {
                sb.AppendFormat("<li class='list-group-item' data-section-id='{0}'><span class='section-text'>{1}</span></li>", 
                    section.Id, section.Name);
            }
            return sb.ToString();
        }

        private string GenerateSectionDetailsHtml(AuditSection section)
        {
            var sb = new StringBuilder();
            if (section != null)
            {
                sb.AppendFormat("<div class='section' data-section-id='{0}'>", section.Id);
                sb.Append("<div class='text-center mb-4'>");
                sb.AppendFormat("<h4 class='section-name'>{0}</h4>", section.Name);
                sb.Append("</div>");
                sb.Append("<div class='questions'>");

                int questionNumber = 1;
                foreach (var question in section.Questions)
                {
                    AppendQuestion(sb, question, questionNumber++);
                }

                sb.Append("</div>");
                sb.Append("<div class='text-end mt-3'>");
                sb.Append("<button class='btn btn-primary btn-sm add-question-btn'>Add Question</button>");
                sb.Append("</div>");
                sb.Append("</div>");
            }
            return sb.ToString();
        }
        
        private void AppendQuestion(StringBuilder sb, AuditQuestion question, int questionNumber)
        {
            sb.AppendFormat("<div class='question mb-3 pb-3 border-bottom' data-question-id='{0}'>", question.Id);
            
            sb.Append("<div class='d-flex justify-content-between align-items-center'>");
            sb.AppendFormat("<h5 class='question-text mb-0'>{0}</h5>", question.Text);
            sb.Append("<div>");
            sb.Append("<a href='#' class='edit-question me-2'>Edit</a>");
            sb.Append("<a href='#' class='delete-question'>Delete</a>");
            sb.Append("</div>");
            sb.Append("</div>");

    
            // Radio Buttons
            AppendRadioButtons(sb, question);
    
            // Attachment and Comments buttons
            AppendAttachmentAndCommentButtons(sb);
    
            // Comments accordion
            AppendCommentsAccordion(sb);

            sb.Append("</div>");
        }

        private void AppendRadioButtons(StringBuilder sb, AuditQuestion question)
        {
            sb.Append("<div class='response-options'>");
            foreach (var option in Enum.GetValues(typeof(RadioResponse)))
            {
                string displayText = option.ToString();
                if (displayText == "RequiresImprovement")
                {
                    displayText = "Requires Improvement";
                } 
                else if (displayText == "NotAssessed")
                {
                    displayText = "Not Assessed";   
                }
                
                sb.AppendFormat(@"
                    <div class='form-check form-check-inline'>
                        <input class='form-check-input' type='radio' name='response-{0}' 
                            id='response-{0}-{1}' value='{1}' onchange='saveChoice(this, {0})'>
                        <label class='form-check-label' for='response-{0}-{1}'>{2}</label>
                    </div>
                ", question.Id, option, displayText);
            }
            sb.Append("</div>");
        }

        private void AppendAttachmentAndCommentButtons(StringBuilder sb)
        {
            sb.Append(@"
                <div class='mt-2 d-flex justify-content-end'>
                    <button class='btn btn-secondary btn-sm attachments-btn me-2'>Attachments</button>
                    <button class='btn btn-secondary btn-sm comments-btn'>Comments</button>
                </div>
            "); 
        }

        private void AppendCommentsAccordion(StringBuilder sb)
        {
            sb.Append(@"
                <div class='comments-accordion mt-2' style='display: none;'>
                    <div class='card'>
                        <div class='card-body'>
                            <textarea class='form-control comment-text' rows='3'></textarea>
                            <button class='btn btn-primary btn-sm mt-2 save-comment'>Save</button>
                        </div>
                    </div>
                </div>
            ");
        }

        public IActionResult OnGetSectionDetails(int sectionId)
        {
            var section = _context.AuditSections
                .Include(s => s.Questions)
                .FirstOrDefault(s => s.Id == sectionId);

            if (section == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                sectionDetailsHtml = GenerateSectionDetailsHtml(section)
            });
        }

        public async Task<IActionResult> OnPostSaveRadioAsync([FromBody] RadioChoice choice)
        {
            Console.WriteLine($"RadioAnswer: {choice.RadioAnswer}, Question Id: {choice.QuestionId}");
            if (choice == null) 
            {
                Console.WriteLine("Null chosen");
                return new JsonResult(new { success = false, message = "Choice cannot be null." });
            }

            Console.WriteLine($"Radio Answer: {choice.RadioAnswer}, Question Id: {choice.QuestionId}");

            try
            {
                RadioResponse parsedResponse;

                if (Enum.TryParse(choice.RadioAnswer, true, out parsedResponse))
                {
                    Console.WriteLine($"Parsed response: {parsedResponse}");
                    var response = await _context.QuestionResponses
                        .FirstOrDefaultAsync(x => x.QuestionId == choice.QuestionId);
                    Console.WriteLine($"Response is: {response}");
                    if (response != null)
                    {
                        response.RadioAnswer = parsedResponse;
                        _context.QuestionResponses.Update(response);
                    }
                    else
                    {
                        var newResponse = new QuestionResponse
                        {
                            QuestionId = choice.QuestionId,
                            RadioAnswer = parsedResponse
                        };
                        _context.QuestionResponses.Add(newResponse);
                    }
                }
                
                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        
        public class RadioChoice
        {
            public string RadioAnswer { get; set; }
            public int QuestionId { get; set; }
        }
        
    }
}
