namespace Shared.DTOs;

public class JobCategoryFilterDto: PaginationRequestDto
{
    public string? SearchText { get; set; }
}
