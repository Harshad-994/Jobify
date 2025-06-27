namespace Shared.DTOs;

public class JobPostingFilterDto : PaginationRequestDto
{
    public string? SearchText { get; set; }
    public string? Title { get; set; }
    public string? CompanyName { get; set; }
    public string? Location { get; set; }
    public Guid? CategoryId { get; set; }
    public int? EmploymentType { get; set; }
    public string? SalaryRange { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime? ClosingDateEnd { get; set; }
    public bool? IsActive { get; set; }
}