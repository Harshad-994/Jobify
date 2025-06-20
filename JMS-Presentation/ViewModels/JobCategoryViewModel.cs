using System.ComponentModel.DataAnnotations;

namespace JMS_Presentation.ViewModels;

public class JobCategoryViewModel
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = null!;
    public int TotalNoOfJobs { get; set; }
}
