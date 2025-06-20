using System.ComponentModel.DataAnnotations;

namespace DAL.Data.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int Role { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ResumeUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
