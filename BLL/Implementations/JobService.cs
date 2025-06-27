using BLL.Interfaces;
using DAL.Data.Models;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.DTOs;
using Shared.Exceptions;

namespace BLL.Implementations;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly ILogger<JobService> _logger;
    public JobService(ILogger<JobService> logger, IJobRepository jobRepository, IJobApplicationRepository jobApplicationRepository)
    {
        _logger = logger;
        _jobRepository = jobRepository;
        _jobApplicationRepository = jobApplicationRepository;
    }

    public async Task<bool> CreateAsync(JobPostingDto jobPosting)
    {
        if (jobPosting == null)
        {
            _logger.LogWarning("Job posting cannot be null.");
            throw new ArgumentNullException(nameof(jobPosting));
        }
        if (!(jobPosting.ClosingDate > DateOnly.FromDateTime(DateTime.UtcNow)))
        {
            _logger.LogWarning("Job closing date can't be in the past.");
            throw new InvalidJobClosingDateException(jobPosting.ClosingDate);
        }
        var newJobPosting = new JobPosting
        {
            Title = jobPosting.Title,
            Description = jobPosting.Description,
            CompanyName = jobPosting.CompanyName,
            Location = jobPosting.Location,
            CategoryId = jobPosting.CategoryId,
            EmploymentType = jobPosting.EmploymentType,
            SalaryRange = jobPosting.SalaryRange,
            ClosingDate = jobPosting.ClosingDate,
            IsActive = jobPosting.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        await _jobRepository.AddAsync(newJobPosting);
        _logger.LogInformation("Job posting created successfully: {jobPostingName}", jobPosting.Title);
        return true;
    }

    public async Task<bool> UpdateAsync(JobPostingDto jobPosting)
    {
        if (jobPosting == null)
        {
            _logger.LogWarning("Job posting cannot be null.");
            throw new ArgumentNullException(nameof(jobPosting));
        }

        var existingJobPosting = await _jobRepository.GetByIdAsync(jobPosting.Id);
        if (existingJobPosting == null)
        {
            _logger.LogWarning("Job posting with id {jobId} not found.", jobPosting.Id);
            throw new JobApplicationNotFoundException(jobPosting.Id);
        }

        // if (!jobPosting.IsActive)
        // {
        //     _logger.LogWarning("Job posting is not active.");
        //     throw new JobPostingNotActiveException(jobPosting.Id);
        // }

        if (!(jobPosting.ClosingDate > DateOnly.FromDateTime(DateTime.UtcNow)) || jobPosting.ClosingDate < DateOnly.FromDateTime(jobPosting.CreatedAt))
        {
            _logger.LogWarning("Job closing date can't be in the past.");
            throw new InvalidJobClosingDateException(existingJobPosting.ClosingDate);
        }

        if (DateOnly.FromDateTime(DateTime.UtcNow) > jobPosting.ClosingDate)
        {
            _logger.LogWarning("Job posting has already closed.");
            throw new JobPostingExpiredException(jobPosting.Id, jobPosting.ClosingDate);
        }

        existingJobPosting.Title = jobPosting.Title;
        existingJobPosting.Description = jobPosting.Description;
        existingJobPosting.CompanyName = jobPosting.CompanyName;
        existingJobPosting.Location = jobPosting.Location;
        existingJobPosting.CategoryId = jobPosting.CategoryId;
        existingJobPosting.EmploymentType = jobPosting.EmploymentType;
        existingJobPosting.SalaryRange = jobPosting.SalaryRange;
        existingJobPosting.ClosingDate = jobPosting.ClosingDate;
        existingJobPosting.IsActive = jobPosting.IsActive;
        existingJobPosting.UpdatedAt = DateTime.UtcNow;
        await _jobRepository.UpdateAsync(existingJobPosting);
        _logger.LogInformation("Job posting updated successfully: {jobPostingId}", jobPosting.Id);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid jobId)
    {
        if (jobId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(jobId));
        }
        var jobPosting = await _jobRepository.GetByIdAsync(jobId);
        if (jobPosting == null)
        {
            _logger.LogWarning("Job posting with id {jobId} not found.", jobId);
            throw new JobApplicationNotFoundException(jobId);
        }

        var applications = _jobApplicationRepository.GetAll().Include(j => j.JobPosting);
        var applicationCount = applications.Count(a => a.JobPosting.Id == jobId);
        if (applicationCount > 0)
        {
            _logger.LogWarning("Cannot delete job with active applications: {jobId}", jobId);
            throw new DeleteJobWithApplicationException(jobId);
        }

        jobPosting.IsDeleted = true;
        jobPosting.UpdatedAt = DateTime.UtcNow;
        await _jobRepository.DeleteAsync(jobPosting);
        _logger.LogInformation("Job posting deleted successfully: {jobPostingId}", jobId);
        return true;
    }

    public JobPostingDto GetById(Guid jobId)
    {
        if (jobId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(jobId));
        }
        var jobPosting = _jobRepository.GetAll().Include(j => j.JobCategory).FirstOrDefault(j => j.Id == jobId);
        if (jobPosting == null)
        {
            _logger.LogWarning("Job posting not found : {jobPostingId}", jobId);
            throw new JobApplicationNotFoundException(jobId);
        }
        return new JobPostingDto
        {
            Id = jobPosting.Id,
            Title = jobPosting.Title,
            Description = jobPosting.Description,
            CompanyName = jobPosting.CompanyName,
            Location = jobPosting.Location,
            CategoryId = jobPosting.CategoryId,
            EmploymentType = jobPosting.EmploymentType,
            SalaryRange = jobPosting.SalaryRange,
            ClosingDate = jobPosting.ClosingDate,
            IsActive = jobPosting.IsActive,
            CreatedAt = jobPosting.CreatedAt,
            UpdatedAt = jobPosting.UpdatedAt,
            CategoryName = jobPosting.JobCategory.Name,
        };
    }

    public async Task<bool> ToggleJobStatus(Guid jobId)
    {
        if (jobId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(jobId));
        }
        var jobPosting = await _jobRepository.GetByIdAsync(jobId) ?? throw new Exception("Job posting not found.");
        jobPosting.IsActive = !jobPosting.IsActive;
        jobPosting.UpdatedAt = DateTime.UtcNow;
        await _jobRepository.UpdateAsync(jobPosting);
        _logger.LogInformation("Job posting status toggled successfully: {jobPostingId}", jobId);
        return true;
    }

    public async Task<PaginationResponseDto<JobPostingDto>> GetJobPostingsAsync(JobPostingFilterDto filter)
    {
        var query = _jobRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var term = filter.SearchText.ToLower().Trim();
            query = query.Where(j =>
                j.SalaryRange.ToLower().Contains(term) ||
                j.Title.ToLower().Contains(term) ||
                j.Description.ToLower().Contains(term) ||
                j.CompanyName.ToLower().Contains(term) ||
                j.Location.ToLower().Contains(term));
        }

        if (filter.CategoryId != null)
            query = query.Where(j => j.CategoryId == filter.CategoryId.Value);
        if (filter.EmploymentType != null)
            query = query.Where(j => j.EmploymentType == filter.EmploymentType.Value);
        if (filter.IsActive == true)
            query = query.Where(j => j.IsActive);
        if (filter.ClosingDateEnd != null)
            query = query.Where(j => j.ClosingDate >= DateOnly.FromDateTime(filter.ClosingDateEnd.Value));

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(j => j.JobCategory)
            .OrderByDescending(j => j.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(j => new JobPostingDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                CompanyName = j.CompanyName,
                Location = j.Location,
                CategoryId = j.CategoryId,
                CategoryName = j.JobCategory.Name,
                EmploymentType = j.EmploymentType,
                SalaryRange = j.SalaryRange,
                ClosingDate = j.ClosingDate,
                IsActive = j.IsActive,
                IsDeleted = j.IsDeleted,
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt
            })
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} jobs out of {Total} total", items.Count, totalCount);

        return new PaginationResponseDto<JobPostingDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

    }
}
