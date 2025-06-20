using BLL.Interfaces;
using JMS_Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace JMS_Presentation.Controllers;

public class CandidateController : Controller
{
    private readonly ICandidateService _candidateService;
    public CandidateController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpGet]
    public async Task<IActionResult> AllCandidates(CandidateFilterDto candidateFilterDto)
    {
        var candidatesDto = await _candidateService.GetCandidatesAsync(candidateFilterDto);
        var candidates = candidatesDto.Items.Select(candidate => new CandidateProfileViewModel
        {
            Id = candidate.Id,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Email = candidate.Email,
            Role = candidate.Role,
            IsActive = candidate.IsActive,
            ResumeUrl = candidate.ResumeUrl,
            UpdatedAt = candidate.UpdatedAt,
            CreatedAt = candidate.CreatedAt,
        }).ToList();
        return View("AllCandidates", candidates);
    }

    [HttpGet]
    public async Task<PaginationResponseDto<CandidateProfileViewModel>> GetAllCandidates(CandidateFilterDto candidateFilterDto)
    {
        var candidatesDto = await _candidateService.GetCandidatesAsync(candidateFilterDto);
        var candidates = candidatesDto.Items.Select(candidate => new CandidateProfileViewModel
        {
            Id = candidate.Id,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Email = candidate.Email,
            Role = candidate.Role,
            IsActive = candidate.IsActive,
            ResumeUrl = candidate.ResumeUrl,
            UpdatedAt = candidate.UpdatedAt,
            CreatedAt = candidate.CreatedAt,
        }).ToList();

        var paginationResponse = new PaginationResponseDto<CandidateProfileViewModel>
        {
            Items = candidates,
            TotalCount = candidatesDto.TotalCount,
            Page = candidatesDto.Page,
            PageSize = candidatesDto.PageSize,
        };

        return paginationResponse;
    }

    [HttpPost]
    public async Task<IActionResult> ToggleCandidateActiveStatus(Guid candidateId)
    {
        var candidateEntity = await _candidateService.ToggleCandidateStatus(candidateId);
        return Ok(new { Success = true, Message = "Candidate status toggled successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetCandidateProfile(Guid candidateId)
    {
        var candidateProfile = await _candidateService.GetCandidateProfile(candidateId);
        return View("CandidateProfile", candidateProfile);
    }

    [HttpGet]
    public async Task<IActionResult> CandidateDetails(Guid candidateId)
    {
        var candidateProfileDto = await _candidateService.GetCandidateProfile(candidateId);
        var candidateProfile = new CandidateProfileViewModel
        {
            Id = candidateProfileDto.Id,
            FirstName = candidateProfileDto.FirstName,
            LastName = candidateProfileDto.LastName,
            Email = candidateProfileDto.Email,
            Role = candidateProfileDto.Role,
            IsActive = candidateProfileDto.IsActive,
            ResumeUrl = candidateProfileDto.ResumeUrl,
            UpdatedAt = candidateProfileDto.UpdatedAt,
            CreatedAt = candidateProfileDto.CreatedAt,
        };

        var jobApplicationsDto = candidateProfileDto.JobApplications;

        var jobApplications = jobApplicationsDto.Select(jobApplication => new JobApplicationViewModel
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
            JobApplicationStatusName = jobApplication.JobApplicationStatusName
        }).ToList();
        candidateProfile.JobApplications = jobApplications;
        
        return View("CandidateDetails", candidateProfile);
    }

}
