using System.ComponentModel.DataAnnotations;

namespace JMS_Presentation.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    public string CurrentPassword { get; set; } = null!;

    [Required]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,255}$", ErrorMessage = "Please enter password with minimum length of 8 to 255 with at least one uppercase letter, one lowercase letter, one number, and one special character. ")]
    public string NewPassword { get; set; } = null!;

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Both passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}
