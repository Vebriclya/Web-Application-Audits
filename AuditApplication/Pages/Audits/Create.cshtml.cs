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
            Console.WriteLine($"Number of templates loaded: {AvailableTemplates.Count}");
            foreach (var template in AvailableTemplates)
            {
                Console.WriteLine($"Template ID: {template.Id}, Name: {template.Name}");
            }
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
                    Order = templateSection.Order
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

            var auditHtml = GenerateAuditHtml(audit);

            return new JsonResult(new { success = true, auditHtml = auditHtml });
        }

        public class CreateAuditRequest
        {
            public int TemplateId { get; set; }
        }

        private string GenerateAuditHtml(Audit audit)
        {
                var sb = new StringBuilder();
    
                // Audit Title 
                AppendAuditTitle(sb, audit);
    
                // Main content row
                sb.Append("<div class='row g-0'>");
    
                // Left sidebar
                sb.Append("<div class='col-3 left-sidebar'>");
                AppendSectionList(sb, audit.Sections);
                sb.Append("</div>");
    
                // Right content
                sb.Append("<div class='col-9 right-content'>");
                AppendSectionDetails(sb, audit.Sections.FirstOrDefault());
                sb.Append("</div>");
    
                sb.Append("</div>");
    
                return sb.ToString();
        }

        private void AppendAuditTitle(StringBuilder sb, Audit audit)
        {
            sb.AppendFormat(@"
                <div class='row header-area m-0'>
                    <div class='col-12 d-flex justify-content-between align-items-center'>
                        <div class='col-2 text-start'>
                            <a href='/Audits/Index'><-- Back to List</a>
                        </div>
                        <div class='col-2'></div>
                        <h3 id='auditTitle' class='col-6 text-center m-0'>{0}</h3>
                        <div class='col-2 text-end'>
                            <a href='#' id='editAuditName'>Edit</a>
                        </div>
                    </div>
                </div>
            ", audit.AuditName);
        }

        private void AppendSectionList(StringBuilder sb, IEnumerable<AuditSection> sections)
        {
            sb.Append("<ul class='list-group' id='sectionList'>");
            foreach (var section in sections)
            {
                sb.AppendFormat("<li class='list-group-item' data-section-id='{0}'>{1}</li>", 
                    section.Id, section.Name);
            }
            sb.Append("</ul>");
        }

        private void AppendSectionDetails(StringBuilder sb, AuditSection section)
        {
            sb.Append("<div id='sectionDetails' class='p-3'>");
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
            sb.Append("</div>");
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
                            id='response-{0}-{1}' value='{1}'>
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
        
    }
}
