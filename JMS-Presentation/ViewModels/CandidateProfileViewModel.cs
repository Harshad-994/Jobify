namespace JMS_Presentation.ViewModels;

public class CandidateProfileViewModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Role { get; set; }
    public bool IsActive { get; set; }
    public string? ResumeUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<JobApplicationViewModel> JobApplications { get; set; } = new List<JobApplicationViewModel>();
}
