﻿namespace AuditApplication.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int Order { get; set; }
    public int SectionId { get; set; }
    public Section Section { get; set; }
}