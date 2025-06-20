using BLL.Interfaces;
using DAL.Data.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.DTOs;

namespace BLL.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(ILogger<DashboardService> logger, IJobApplicationRepository jobApplicationRepository, IJobRepository jobRepository, ICandidateRepository candidateRepository)
    {
        _logger = logger;
        _jobApplicationRepository = jobApplicationRepository;
        _jobRepository = jobRepository;
        _candidateRepository = candidateRepository;
    }

    public async Task<AdminDashboardStatsDto> GetAdminDashboardStats()
    {
        var totalNoOfJobs = await _jobRepository.GetAll().CountAsync();
        var totalNoOfCandidates = await _candidateRepository.GetAll().Where(u => u.Role == (int)Role.Candidate).CountAsync();
        var totalNoOfApplications = await _jobApplicationRepository.GetAll().CountAsync();
        var recentJobPostings = await _jobRepository.GetAll().Include(j=>j.JobCategory).OrderByDescending(j => j.CreatedAt).Take(3).ToListAsync();
        var recentJobPostingsDto = recentJobPostings.Select(j => new JobPostingDto
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
            UpdatedAt = j.UpdatedAt,
            TotalNoOfApplications = _jobApplicationRepository.GetAll().Where(a => a.JobPostingId == j.Id).CountAsync().Result,
        }).ToList();

        _logger.LogInformation("Admin dashboard stats retrieved successfully");

        return new AdminDashboardStatsDto
        {
            TotalNoOfJobs = totalNoOfJobs,
            TotalNoOfCandidates = totalNoOfCandidates,
            TotalNoOfApplications = totalNoOfApplications,
            RecentJobPostings = recentJobPostingsDto
        };
    }
}
