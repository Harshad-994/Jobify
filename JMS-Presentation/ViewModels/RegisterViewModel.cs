using System.ComponentModel.DataAnnotations;

namespace JMS_Presentation.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Firstname is required.")]
    [MaxLength(100,ErrorMessage = "First Name cannot exceed 100 characters.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last Name is required.")]
    [MaxLength(100,ErrorMessage ="Lastname cannot exceed 100 characters.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[0-9a-zA-Z.-]+@[a-zA-Z-.0-9]+\.[a-zA-Z]{2,5}$", ErrorMessage = "Please enter email in correct format.")]
    [MaxLength(255,ErrorMessage = "Email cannot exceed 255 characters.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,255}$", ErrorMessage = "Please enter password with minimum length of 8 to 255 with at least one uppercase letter, one lowercase letter, one number, and one special character. ")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirm Password is required. ")]
    [Compare(nameof(Password), ErrorMessage = "Both passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}
