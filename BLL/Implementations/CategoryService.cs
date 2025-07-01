using BLL.Interfaces;
using DAL.Data.Models;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.DTOs;
using Shared.Exceptions;

namespace BLL.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ILogger<CategoryService> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IJobRepository _jobRepository;
    public CategoryService(ILogger<CategoryService> logger, ICategoryRepository categoryRepository, IJobRepository jobRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _jobRepository = jobRepository;
    }

    public async Task<bool> CreateAsync(JobCategoryDto category)
    {
        if (await CategoryExistsByNameAsync(category.Name))
        {
            _logger.LogWarning("Category with name {categoryName} already exists.", category.Name);
            throw new DuplicateCategoryNameException(category.Name);
        }
        var categoryEntity = new JobCategory
        {
            Name = category.Name,
            Description = category.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _categoryRepository.AddAsync(categoryEntity);
        _logger.LogInformation("Category with name {categoryName} created successfully", category.Name);
        return true;
    }

    public async Task<bool> UpdateAsync(JobCategoryDto category)
    {
        var categoryEntity = await _categoryRepository.GetByIdAsync(category.Id);
        if (categoryEntity == null)
        {
            _logger.LogWarning("Category with id {categoryId} not found.", category.Id);
            throw new JobCategoryNotFoundException(category.Id);
        }

        if (await CategoryExistsByNameAsync(category.Name, category.Id))
        {
            _logger.LogWarning("Category with name {categoryName} already exists.", category.Name);
            throw new DuplicateCategoryNameException(category.Name);
        }

        categoryEntity.Name = category.Name;
        categoryEntity.Description = category.Description;
        categoryEntity.UpdatedAt = DateTime.UtcNow;
        await _categoryRepository.UpdateAsync(categoryEntity);
        _logger.LogInformation("Category with id {categoryId} updated successfully", category.Id);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid categoryId)
    {
        var categoryEntity = await _categoryRepository.GetByIdAsync(categoryId);
        if (categoryEntity == null)
        {
            _logger.LogWarning("Category with id {categoryId} not found.", categoryId);
            throw new JobCategoryNotFoundException(categoryId);
        }
        if (await _categoryRepository.HasActiveJobsAsync(categoryId))
        {
            throw new CategoryHasActiveJobsException(categoryId);
        }

        await _categoryRepository.DeleteAsync(categoryEntity);
        _logger.LogInformation("Category with id {categoryId} deleted successfully", categoryId);
        return true;
    }

    public async Task<bool> CategoryExistsByNameAsync(string name, Guid? id = null)
    {
        return await _categoryRepository.ExistsByNameAsync(name, id);
    }

    // public async Task<bool> HasActiveJobsAsync(Guid categoryId)
    // {
    //     try
    //     {
    //         var category = await _categoryRepository.GetByIdAsync(categoryId) ?? throw new Exception("Category not found.");
    //         return await _categoryRepository.HasActiveJobsAsync(categoryId);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error occurred while checking if category has active jobs.");
    //         throw;
    //     }
    // }

    public async Task<PaginationResponseDto<JobCategoryDto>> GetJobCategoriesAsync(JobCategoryFilterDto filter)
    {
        var query = _categoryRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var term = filter.SearchText.ToLower().Trim();
            query = query.Where(c =>
            EF.Functions.ILike(c.Name, $"%{term}%") ||
            EF.Functions.ILike(c.Description, $"%{term}%"));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip(filter.PageSize != 0 ? (filter.Page - 1) * filter.PageSize : 0)
            .Take(filter.PageSize != 0 ? filter.PageSize : totalCount)
            .Select(c => new JobCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                TotalNoOfJobs = c.JobPostings.Count(),
            })
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} job categories out of {Total} total", items.Count, totalCount);

        return new PaginationResponseDto<JobCategoryDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

    }

    public async Task<JobCategoryDto> GetByIdAsync(Guid categoryId)
    {
        var category = await _categoryRepository.GetAll().Where(c => c.Id == categoryId).
        Select(c => new JobCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            TotalNoOfJobs = c.JobPostings.Count(),
        }).SingleOrDefaultAsync();

        if (category == null)
        {
            _logger.LogWarning("Category with id {categoryId} not found.", categoryId);
            throw new JobCategoryNotFoundException(categoryId);
        }

        return category;
    }
}
