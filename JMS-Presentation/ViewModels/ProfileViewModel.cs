using System.ComponentModel.DataAnnotations;

namespace JMS_Presentation.ViewModels;

public class ProfileViewModel
{
    public Guid UserId { get; set; }
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
}
