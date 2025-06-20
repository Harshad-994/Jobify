namespace Shared.DTOs;

public class JobApplicationFilterDto : PaginationRequestDto
{
    public string? SearchText { get; set; }
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public Guid? JobPostingId { get; set; }
    public int? ApplicationStatus { get; set; }
    public Guid? UserId { get; set; }
}
