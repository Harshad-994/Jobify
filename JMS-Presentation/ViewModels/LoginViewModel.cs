using System.ComponentModel.DataAnnotations;

namespace JMS_Presentation.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[0-9a-zA-Z.-]+@[a-zA-Z-.0-9]+\.[a-zA-Z]{2,5}$", ErrorMessage = "Please enter email in correct format.")]
    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; } = false;
}
