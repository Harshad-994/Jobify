using Shared.DTOs;

namespace BLL.Interfaces;

public interface ICategoryService
{
    Task<bool> CreateAsync(JobCategoryDto category);
    Task<bool> UpdateAsync(JobCategoryDto category);
    Task<bool> DeleteAsync(Guid categoryId);
    Task<bool> CategoryExistsByNameAsync(string name,Guid? id);
    // Task<bool> HasActiveJobsAsync(Guid categoryId);
    Task<JobCategoryDto> GetByIdAsync(Guid categoryId);
    Task<PaginationResponseDto<JobCategoryDto>> GetJobCategoriesAsync(JobCategoryFilterDto filter);
}
