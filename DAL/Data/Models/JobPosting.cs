
namespace DAL.Data.Models;

public class JobPosting
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string Location { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public int EmploymentType { get; set; }
    public string SalaryRange { get; set; } = null!;
    public DateOnly ClosingDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    public virtual JobCategory JobCategory { get; set; } = null!;
}
