using BLL.Interfaces;
using DAL.Data.Enums;
using DAL.Interfaces;
using JMS_Presentation.SignalR;
using JMS_Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.DTOs;
using Shared.Models;

namespace JMS_Presentation.Controllers;

[Authorize]
public class JobApplicationController : Controller
{
    private readonly IJobApplicationService _jobApplicationService;
    private readonly ICandidateService _candidateService;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<NotificationHub> _hubContext;
    public JobApplicationController(IJobApplicationService jobApplicationService, ICandidateService candidateService, IHubContext<NotificationHub> hubContext, INotificationService notificationService)
    {
        _jobApplicationService = jobApplicationService;
        _candidateService = candidateService;
        _hubContext = hubContext;
        _notificationService = notificationService;
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
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
            JobApplicationStatusName = jobApplication.JobApplicationStatusName,
            CompanyName = jobApplication.CompanyName
        }).ToList();
        return View("AllJobApplications", jobApplications);
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin) + "," + nameof(Role.Candidate))]
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
            JobApplicationStatusName = jobApplication.JobApplicationStatusName,
            CompanyName = jobApplication.CompanyName
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
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> DeleteApplication(Guid applicationId)
    {
        var jobApplication = await _jobApplicationService.DeleteApplicationAsync(applicationId);
        var notificationDto = new NotificationDto
        {
            Title = "Application Deleted",
            Message = $"Your application with title {jobApplication.Title} at {jobApplication.CompanyName} has been deleted by admin.",
            UserId = jobApplication.UserId
        };
        var notification = await _notificationService.CreateNotificationAsync(notificationDto);

        await _hubContext.Clients.User(jobApplication.UserId.ToString()).SendAsync("ReceiveNotification", notification);
        return Ok(new SuccessResponce { Success = true, Message = "Application deleted successfully." });
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> UpdateApplicationStatus(Guid applicationId, int newStatus)
    {
        var jobApplication = await _jobApplicationService.UpdateApplicationStatusAsync(applicationId, newStatus);
        return Ok(new SuccessResponce { Success = true, Message = "Application status updated." });
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Candidate))]
    public async Task<IActionResult> WithdrawApplication(Guid applicationId)
    {
        var jobApplication = await _jobApplicationService.UpdateApplicationStatusAsync(applicationId, (int)ApplicationStatus.Withdrawn);
        return Ok(new SuccessResponce { Success = true, Message = "Application withdrew successfully." });
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin) + "," + nameof(Role.Candidate))]
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
            ResumeUrl = jobApplication.ResumeUrl,
            CompanyName = jobApplication.CompanyName
        };
        return jobApplicationViewModel;
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Candidate))]
    public async Task<IActionResult> AllJobApplicationsForCandidate()
    {
        var userId = User.GetUserId();
        if (Guid.Empty == userId)
        {
            return RedirectToAction("Login");
        }

        var jobApplicationsDto = await _jobApplicationService.GetAllApplicationsOfCandidateAsync(userId);

        var jobApplications = jobApplicationsDto.Select(jobApplication => new JobApplicationViewModel
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
            JobApplicationStatusName = jobApplication.JobApplicationStatusName,
            CompanyName = jobApplication.CompanyName
        }).ToList();
        return View("AllJobApplicationsForCandidate", jobApplications);
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Candidate))]
    public async Task<IActionResult> ApplyForJob(JobApplicationViewModel jobApplication)
    {
        var userId = User.GetUserId();
        if (Guid.Empty == userId)
        {
            return RedirectToAction("Login");
        }

        var jobApplicationDto = new JobApplicationDto
        {
            UserId = userId,
            JobPostingId = jobApplication.JobPostingId,
            CoverLetter = jobApplication.CoverLetter
        };
        await _jobApplicationService.ApplyAsync(jobApplicationDto);
        return Ok(new SuccessResponce { Success = true, Message = "Application submitted successfully." });
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Candidate))]
    public async Task<IActionResult> GetJobApplicationFormAsync(Guid jobId)
    {
        var userId = User.GetUserId();
        if (Guid.Empty == userId)
        {
            return RedirectToAction("Login");
        }
        var user = await _candidateService.GetCandidateProfile(userId);
        return PartialView("_ApplyForm", new JobApplicationViewModel() { JobPostingId = jobId, ResumeUrl = user.ResumeUrl });
    }
}
