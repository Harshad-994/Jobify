using BLL.Interfaces;
using DAL.Interfaces;
using JMS_Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace JMS_Presentation.Controllers;

public class JobApplicationController : Controller
{
    private readonly IJobApplicationService _jobApplicationService;
    public JobApplicationController(IJobApplicationService jobApplicationService)
    {
        _jobApplicationService = jobApplicationService;
    }

    [HttpGet]
    public async Task<IActionResult> AllJobApplications(JobApplicationFilterDto jobApplicationFilterDto)
    {
        var jobApplicationsDto = await _jobApplicationService.GetJobApplicationsAsync(jobApplicationFilterDto);
        var jobApplications = jobApplicationsDto.Items.Select(jobApplication => new JobApplicationViewModel
        {
            Id = jobApplication.Id,
            UserId = jobApplication.UserId,
            Title = jobApplication.Title,
            FirstName = jobApplication.FirstName,
            LastName = jobApplication.LastName,
            JobPostingId = jobApplication.JobPostingId,
            CoverLetter = jobApplication.CoverLetter,
            AppliedAt = jobApplication.AppliedAt,
            UpdatedAt = jobApplication.UpdatedAt,
            ApplicationStatus = jobApplication.ApplicationStatus,
            JobApplicationStatusName = jobApplication.JobApplicationStatusName
        }).ToList();
        return View("AllJobApplications", jobApplications);
    }

    [HttpGet]
    public async Task<PaginationResponseDto<JobApplicationViewModel>> GetAllJobApplications(JobApplicationFilterDto jobApplicationFilterDto)
    {
        var jobApplicationsDto = await _jobApplicationService.GetJobApplicationsAsync(jobApplicationFilterDto);
        var jobApplications = jobApplicationsDto.Items.Select(jobApplication => new JobApplicationViewModel
        {
            Id = jobApplication.Id,
            UserId = jobApplication.UserId,
            Title = jobApplication.Title,
            FirstName = jobApplication.FirstName,
            LastName = jobApplication.LastName,
            JobPostingId = jobApplication.JobPostingId,
            CoverLetter = jobApplication.CoverLetter,
            AppliedAt = jobApplication.AppliedAt,
            UpdatedAt = jobApplication.UpdatedAt,
            ApplicationStatus = jobApplication.ApplicationStatus,
            JobApplicationStatusName = jobApplication.JobApplicationStatusName
        }).ToList();
        var paginationResponse = new PaginationResponseDto<JobApplicationViewModel>
        {
            Items = jobApplications,
            TotalCount = jobApplicationsDto.TotalCount,
            Page = jobApplicationFilterDto.Page,
            PageSize = jobApplicationFilterDto.PageSize
        };
        return paginationResponse;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteApplication(Guid applicationId)
    {
        var jobApplication = await _jobApplicationService.DeleteApplicationAsync(applicationId);
        return Ok(new { Success = true, Message = "Application deleted successfully." });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateApplicationStatus(Guid applicationId, int newStatus)
    {
        var jobApplication = await _jobApplicationService.UpdateApplicationStatusAsync(applicationId, newStatus);
        return Ok(new { Success = true, Message = "Application status updated." });
    }

    [HttpGet]
    public async Task<JobApplicationViewModel> GetJobApplication(Guid applicationId)
    {
        var jobApplication = await _jobApplicationService.GetApplicationByIdAsync(applicationId);
        var jobApplicationViewModel = new JobApplicationViewModel
        {
            Id = jobApplication.Id,
            UserId = jobApplication.UserId,
            Title = jobApplication.Title,
            FirstName = jobApplication.FirstName,
            JobPostingId = jobApplication.JobPostingId,
            CoverLetter = jobApplication.CoverLetter,
            AppliedAt = jobApplication.AppliedAt,
            UpdatedAt = jobApplication.UpdatedAt,
            ApplicationStatus = jobApplication.ApplicationStatus,
            ResumeUrl = jobApplication.ResumeUrl
        };
        return jobApplicationViewModel;
    }
}
