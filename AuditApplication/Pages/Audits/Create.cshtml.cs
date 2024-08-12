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
    public class CreateModel : PageModel
    {
        private readonly AuditApplication.Data.AuditContext _context;
        
        public List<AuditTemplate> AvailableTemplates { get;set; }

        public async Task OnGetAsync()
        {
            AvailableTemplates = await _context.AuditTemplates.ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateAuditFromTemplateAsync(int templateId)
        {
            var template = await _context.AuditTemplates
                .Include(t => t.Sections)
                .ThenInclude(s => s.Questions)
                .FirstOrDefaultAsync(t => t.Id == templateId);

            if (template == null)
            {
                return new JsonResult(new { success = false });
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

        private string GenerateAuditHtml(Audit audit)
        {
            var sb = new StringBuilder();
            sb.Append("<div class=\"container-fluid\">");
            
            // Title
            sb.Append("<div class=\"row mb-3\">");
            sb.Append("<div class=\"col-12\">");
            sb.AppendFormat("<h3 id=\"auditTitle\">{0}</h3>", audit.AuditName);
            sb.Append("<a href=\"#\" id=\"editAuditName\">Edit</a>");
            sb.Append("</div></div>");
            
            // Create two-columns
            sb.Append("<div class=\"row\">");
            
            // Left column: Section list
            sb.Append("<div class=\"col-md-4\">");
            sb.Append("<ul class=\"list-group\" id=\"sectionList\">");
            foreach (var section in audit.Sections)
            {
                sb.AppendFormat("<li class=\"list-group-item\" data-section-id=\"{0}\">{1}</li>", 
                    section.Id, section.Name);
            }
            sb.Append("</ul></div>");
            
            // Right column: Section display
            sb.Append("<div class=\"col-md-8\"><div id=\"sectionDetails\">");
            var firstSection = audit.Sections.FirstOrDefault();
            if (firstSection != null)
            {
                sb.AppendFormat("<div class=\"section\" data-section-id=\"{0}\">", firstSection.Id);
                sb.AppendFormat("<h4 class=\"section-name\">{0}</h4>", firstSection.Name);
                sb.Append("<a href=\"#\" class=\"rename-section\">Rename</a>");
                sb.Append("<div class=\"questions\">");

                foreach (var question in firstSection.Questions)
                {
                    AppendQuestion(sb, question);
                }

                sb.Append("</div>");
                sb.Append("<button class=\"btn btn-primary btn-sm mt-3 add-question\">Add Question</button>");
                sb.Append("</div>");
            }

            sb.Append("</div></div>");
            sb.Append("</div></div>");
            
            return sb.ToString();
        }

        private void AppendQuestion(StringBuilder sb, AuditQuestion question)
        {
            sb.AppendFormat("<div class=\"question\" data-question-id=\"{0}\">", question.Id);
            sb.AppendFormat("<p class=\"question-text\">{0}</p>", question.Text);
            sb.Append("<a href=\"#\" class=\"rename-question\">Rename</a>");
            sb.Append("<a href=\"#\" class=\"delete-question\">Delete</a>");
            
            //Radio Buttons
            sb.Append("<div class=\"response-options\">");
            foreach (var option in Enum.GetValues(typeof(RadioResponse)))
            {
                sb.AppendFormat(
                    "<div class=\"form-check form-check-inline\">" +
                    "<input class=\"form-check-input\" type=\"radio\" name=\"response-{0}\" " +
                    "id=\"response-{0}-{1}\" value=\"{1}\">" +
                    "<label class=\"form-check-label\" for=\"response-{0}-{1}\">{1}</label>" +
                    "</div>", 
                    question.Id, option);
            }
            sb.Append("</div>");
            
            // Attachment and Comments buttons
            sb.Append("<div class=\"mt-2\">");
            sb.Append("<button class=\"btn btn-secondary btn-sm attachments-btn\">Attachments</button>");
            sb.Append("<button class=\"btn btn-secondary btn-sm comments-btn\">Comments</button>");
            sb.Append("</div>");
            
            // Comments accordion
            sb.Append("<div class=\"comments-accordion mt-2\" style=\"display: none;\">");
            sb.Append("<div class=\"card\"><div class=\"card-body\">");
            sb.Append("<textarea class=\"form-control comment-text\" rows=\"3\"></textarea>");
            sb.Append("<button class=\"btn btn-primary btn-sm mt-2 save-comment\">Save</button>");
            sb.Append("</div></div></div>");

            sb.Append("</div>");
        }

    }
}
