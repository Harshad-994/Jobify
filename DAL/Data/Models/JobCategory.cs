namespace DAL.Data.Models;

public class JobCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
}
