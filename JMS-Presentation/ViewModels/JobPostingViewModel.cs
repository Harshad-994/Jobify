using System.ComponentModel.DataAnnotations;

namespace JMS_Presentation.ViewModels;

public class JobPostingViewModel
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200,ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(2000,ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string Description { get; set; } = null!;
    [Required(ErrorMessage = "CompanyName is required.")]
    [MaxLength(100,ErrorMessage = "CompanyName cannot exceed 100 characters.")]
    public string CompanyName { get; set; } = null!;
    [Required(ErrorMessage = "Location is required.")]
    [MaxLength(200,ErrorMessage = "Location cannot exceed 200 characters.")]
    public string Location { get; set; } = null!;
    [Required(ErrorMessage = "CategoryName is required.")]
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    [Required(ErrorMessage = "Employment type is required.")]
    public int EmploymentType { get; set; }
    [Required(ErrorMessage = "Salary range is required.")]
    [MaxLength(100,ErrorMessage = "Salary range cannot exceed 100 characters.")]
    public string SalaryRange { get; set; } = null!;
    [Required(ErrorMessage = "Closing date is required.")]
    public DateOnly ClosingDate { get; set; }
    public int TotalNoOfApplications { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
