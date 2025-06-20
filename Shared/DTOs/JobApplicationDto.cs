namespace Shared.DTOs;

public class JobApplicationDto
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required Guid JobPostingId { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public required DateTime AppliedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required int ApplicationStatus { get; set; }
    public string? JobApplicationStatusName { get; set; }
}
