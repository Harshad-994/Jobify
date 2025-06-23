namespace JMS_Presentation.ViewModels;

public class JobApplicationViewModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }
    public Guid JobPostingId { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ApplicationStatus { get; set; }
    public string? JobApplicationStatusName { get; set; }
}