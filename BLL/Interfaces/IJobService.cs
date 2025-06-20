using Shared.DTOs;

namespace BLL.Interfaces;

public interface IJobService
{
    Task<bool> CreateAsync(JobPostingDto jobPosting);
    Task<bool> UpdateAsync(JobPostingDto jobPosting);
    Task<bool> DeleteAsync(Guid jobId);
    JobPostingDto GetById(Guid jobId);
    Task<bool> ToggleJobStatus(Guid jobId);
    Task<PaginationResponseDto<JobPostingDto>> GetJobPostingsAsync(JobPostingFilterDto filter);
}
