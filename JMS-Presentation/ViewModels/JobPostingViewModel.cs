using System.ComponentModel.DataAnnotations;

namespace JMS_Presentation.ViewModels;

public class JobPostingViewModel
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = null!;
    [Required(ErrorMessage = "CompanyName is required.")]
    public string CompanyName { get; set; } = null!;
    [Required(ErrorMessage = "Location is required.")]
    public string Location { get; set; } = null!;
    [Required(ErrorMessage = "CategoryName is required.")]
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    [Required(ErrorMessage = "Employment type is required.")]
    public int EmploymentType { get; set; }
    [Required(ErrorMessage = "Salary range is required.")]
    public string SalaryRange { get; set; } = null!;
    [Required(ErrorMessage = "Closing date is required.")]
    public DateOnly ClosingDate { get; set; }
    public int TotalNoOfApplications { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
