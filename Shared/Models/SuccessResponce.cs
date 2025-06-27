namespace Shared.Models;

public class SuccessResponce
{
    public bool Success { get; set; }
    public string? Message { get; set; } = string.Empty;
    public string? RedirectUrl { get; set; } = string.Empty;
}
