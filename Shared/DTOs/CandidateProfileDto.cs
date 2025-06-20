namespace Shared.DTOs;

public class CandidateProfileDto
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public int Role { get; set; }
    public bool IsActive { get; set; }
    public string? ResumeUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<JobApplicationDto> JobApplications { get; set; } = new List<JobApplicationDto>();
}
