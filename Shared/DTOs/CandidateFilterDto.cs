namespace Shared.DTOs;

public class CandidateFilterDto : PaginationRequestDto
{
    public string? SearchText { get; set; }
}
