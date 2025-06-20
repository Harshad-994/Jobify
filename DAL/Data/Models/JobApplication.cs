
namespace DAL.Data.Models;

public class JobApplication
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid JobPostingId { get; set; }
    public string? CoverLetter { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ApplicationStatus { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual JobPosting JobPosting { get; set; } = null!;
}