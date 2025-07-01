using BLL.Interfaces;
using DAL.Data.Enums;
using DAL.Data.Models;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.DTOs;
using Shared.Exceptions;

namespace BLL.Implementations;

public class JobApplicationService : IJobApplicationService
{
    private readonly ILogger<JobApplicationService> _logger;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly ICandidateRepository _userRepository;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    public JobApplicationService(ILogger<JobApplicationService> logger, IJobApplicationRepository jobApplicationRepository, IJobRepository jobRepository, ICandidateRepository candidateRepository, IPasswordEncryptionService passwordEncryptionService)
    {
        _logger = logger;
        _jobApplicationRepository = jobApplicationRepository;
        _jobRepository = jobRepository;
        _userRepository = candidateRepository;
        _passwordEncryptionService = passwordEncryptionService;
    }

    public async Task<bool> ApplyAsync(JobApplicationDto jobApplication)
    {
        var candidateEntity = await _userRepository.GetByIdAsync(jobApplication.UserId);
        if (candidateEntity == null)
        {
            _logger.LogWarning("Candidate with id {CandidateId} not found.", jobApplication.UserId);
            throw new CandidateNotFoundException(jobApplication.UserId);
        }

        var jobPostingEntity = await _jobRepository.GetByIdAsync(jobApplication.JobPostingId);
        if (jobPostingEntity == null)
        {
            _logger.LogWarning("Job posting with id {JobId} not found.", jobApplication.JobPostingId);
            throw new JobPostingNotFoundException(jobApplication.JobPostingId);
        }

        if (await _jobApplicationRepository.ExistsAsync(jobApplication.JobPostingId, jobApplication.UserId))
        {
            _logger.LogWarning("Candidate with id {CandidateId} already applied for job with id {JobId}", jobApplication.UserId, jobApplication.JobPostingId);
            throw new DuplicateApplicationException(jobApplication.UserId, jobApplication.JobPostingId);
        }

        if (!jobPostingEntity.IsActive)
        {
            _logger.LogWarning("Job posting with id {JobId} is not active.", jobApplication.JobPostingId);
            throw new JobPostingNotActiveException(jobApplication.JobPostingId);
        }

        if (DateOnly.FromDateTime(DateTime.UtcNow) > jobPostingEntity.ClosingDate)
        {
            _logger.LogWarning("Job posting with id {JobId} has already closed.", jobApplication.JobPostingId);
            throw new JobPostingExpiredException(jobApplication.JobPostingId, jobPostingEntity.ClosingDate);
        }

        var jobApplicationEntity = new JobApplication
        {
            UserId = jobApplication.UserId,
            JobPostingId = jobApplication.JobPostingId,
            CoverLetter = jobApplication.CoverLetter,
            AppliedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ApplicationStatus = (int)ApplicationStatus.Applied
        };
        await _jobApplicationRepository.AddAsync(jobApplicationEntity);
        _logger.LogInformation("Job application with id {ApplicationId} applied successfully.", jobApplicationEntity.Id);
        return true;
    }

    public async Task<List<JobApplicationDto>> GetAllApplicationsOfCandidateAsync(Guid candidateId)
    {
        var candidateExists = await _userRepository.GetByIdAsync(candidateId);
        if (candidateExists == null)
        {
            _logger.LogWarning("Candidate with id {CandidateId} not found.", candidateId);
            throw new CandidateNotFoundException(candidateId);
        }
        var jobApplicationDtos = await _jobApplicationRepository.GetAll()
                .Where(j => j.UserId == candidateId)
                .Select(j => new JobApplicationDto
                {
                    Id = j.Id,
                    Title = j.JobPosting.Title,
                    UserId = j.UserId,
                    JobPostingId = j.JobPostingId,
                    CoverLetter = j.CoverLetter,
                    AppliedAt = j.AppliedAt,
                    UpdatedAt = j.UpdatedAt,
                    ApplicationStatus = j.ApplicationStatus,
                    CompanyName = j.JobPosting.CompanyName,
                    JobApplicationStatusName = ((ApplicationStatus)j.ApplicationStatus).ToString()
                })
                .ToListAsync();

        return jobApplicationDtos;
    }

    public async Task<bool> UpdateApplicationStatusAsync(Guid applicationId, int newStatus)
    {
        var jobApplicationEntity = await _jobApplicationRepository.GetByIdAsync(applicationId);
        if (jobApplicationEntity == null)
        {
            _logger.LogWarning("Job application {ApplicationId} not found for status update", applicationId);
            throw new JobApplicationNotFoundException(applicationId);
        }

        if (!new List<int> { (int)ApplicationStatus.Applied, (int)ApplicationStatus.UnderReview, (int)ApplicationStatus.Rejected, (int)ApplicationStatus.Offered, (int)ApplicationStatus.InterviewScheduled, (int)ApplicationStatus.Withdrawn }.Contains(newStatus))
        {
            _logger.LogWarning("Invalid application status {NewStatus}.", newStatus);
            throw new InvalidApplicationStatusException(newStatus);
        }

        jobApplicationEntity.ApplicationStatus = newStatus;
        jobApplicationEntity.UpdatedAt = DateTime.UtcNow;
        await _jobApplicationRepository.UpdateAsync(jobApplicationEntity);

        _logger.LogInformation("Job application with id {ApplicationId} updated successfully.", applicationId);
        return true;
    }

    public async Task<JobApplicationDto> DeleteApplicationAsync(Guid applicationId)
    {
        var jobApplicationEntity = await _jobApplicationRepository.GetAll().Where(a => a.Id == applicationId).Include(a => a.JobPosting).SingleOrDefaultAsync();

        if (jobApplicationEntity == null)
        {
            _logger.LogWarning("Job application {ApplicationId} not found for deletion", applicationId);
            throw new JobApplicationNotFoundException(applicationId);
        }

        var jobApplication = new JobApplicationDto
        {
            Id = jobApplicationEntity.Id,
            UserId = jobApplicationEntity.UserId,
            CompanyName = jobApplicationEntity.JobPosting.CompanyName,
            Title = jobApplicationEntity.JobPosting.Title
        };

        await _jobApplicationRepository.DeleteAsync(jobApplicationEntity);
        _logger.LogInformation("Job application with id {ApplicationId} deleted successfully.", applicationId);
        return jobApplication;
    }

    public async Task<PaginationResponseDto<JobApplicationDto>> GetJobApplicationsAsync(JobApplicationFilterDto filter)
    {
        var query = _jobApplicationRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var term = filter.SearchText.ToLower().Trim();
            query = query.Where(j =>
                EF.Functions.ILike(j.User.FirstName, $"%{term}%")
                || EF.Functions.ILike(j.User.LastName, $"%{term}%")
                || EF.Functions.ILike(j.User.FirstName + " " + j.User.LastName, $"%{term}%")
                || EF.Functions.ILike(j.JobPosting.Title, $"%{term}%"));
        }

        if (filter.JobPostingId != null)
            query = query.Where(j => j.JobPostingId == filter.JobPostingId);
        if (filter.ApplicationStatus != null)
            query = query.Where(j => j.ApplicationStatus == filter.ApplicationStatus);
        if (filter.UserId != null)
            query = query.Where(j => j.UserId == filter.UserId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.AppliedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(a => new JobApplicationDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Title = a.JobPosting.Title,
                FirstName = a.User.FirstName,
                LastName = a.User.LastName,
                CompanyName = a.JobPosting.CompanyName,
                JobPostingId = a.JobPostingId,
                CoverLetter = a.CoverLetter,
                AppliedAt = a.AppliedAt,
                UpdatedAt = a.UpdatedAt,
                ApplicationStatus = a.ApplicationStatus,
                JobApplicationStatusName = ((ApplicationStatus)a.ApplicationStatus).ToString()
            })
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} job applications out of {Total} total", items.Count, totalCount);

        return new PaginationResponseDto<JobApplicationDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

    }

    public async Task<JobApplicationDto> GetApplicationByIdAsync(Guid applicationId)
    {
        var jobApplicationEntity = await _jobApplicationRepository.GetAll().Where(j => j.Id == applicationId).Include(j => j.User).Include(j => j.JobPosting).FirstOrDefaultAsync();
        if (jobApplicationEntity == null)
        {
            _logger.LogWarning("Job application {ApplicationId} not found", applicationId);
            throw new JobApplicationNotFoundException(applicationId);
        }
        var jobApplicationDto = new JobApplicationDto
        {
            Id = jobApplicationEntity.Id,
            UserId = jobApplicationEntity.UserId,
            Title = jobApplicationEntity.JobPosting.Title,
            FirstName = jobApplicationEntity.User.FirstName,
            JobPostingId = jobApplicationEntity.JobPostingId,
            CoverLetter = jobApplicationEntity.CoverLetter,
            AppliedAt = jobApplicationEntity.AppliedAt,
            UpdatedAt = jobApplicationEntity.UpdatedAt,
            ApplicationStatus = jobApplicationEntity.ApplicationStatus,
            ResumeUrl = !string.IsNullOrEmpty(jobApplicationEntity.User.ResumeUrl) ? _passwordEncryptionService.DecryptAsync(jobApplicationEntity.User.ResumeUrl).Result : null
        };
        return jobApplicationDto;
    }
}
