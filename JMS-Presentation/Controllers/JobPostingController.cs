using BLL.Interfaces;
using DAL.Data.Models;
using JMS_Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.DTOs;

namespace JMS_Presentation.Controllers;

public class JobPostingController : Controller
{
    private readonly IJobService _jobService;
    private readonly ICategoryService _categoryService;
    public JobPostingController(IJobService jobService, ICategoryService categoryService)
    {
        _jobService = jobService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public IActionResult AddJobPosting()
    {
        var categories = _categoryService.GetAll();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View("AddEditJobPosting", new JobPostingViewModel { });
    }

    [HttpPost]
    public async Task<IActionResult> AddJobPosting(JobPostingViewModel jobPostingViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(jobPostingViewModel);
        }
        var jobPosting = new JobPostingDto
        {
            Title = jobPostingViewModel.Title,
            Description = jobPostingViewModel.Description,
            CompanyName = jobPostingViewModel.CompanyName,
            Location = jobPostingViewModel.Location,
            CategoryId = jobPostingViewModel.CategoryId,
            EmploymentType = jobPostingViewModel.EmploymentType,
            SalaryRange = jobPostingViewModel.SalaryRange,
            ClosingDate = jobPostingViewModel.ClosingDate,
            IsActive = jobPostingViewModel.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _jobService.CreateAsync(jobPosting);
        return Ok(new { Success = true, Message = "Job posting created successfully." });
    }

    [HttpGet]
    public IActionResult EditJobPosting(Guid jobId)
    {
        var jobPostingDto = _jobService.GetById(jobId);
        var jobPosting = new JobPostingViewModel
        {
            Id = jobPostingDto.Id,
            Title = jobPostingDto.Title,
            Description = jobPostingDto.Description,
            CompanyName = jobPostingDto.CompanyName,
            Location = jobPostingDto.Location,
            CategoryId = jobPostingDto.CategoryId,
            EmploymentType = jobPostingDto.EmploymentType,
            SalaryRange = jobPostingDto.SalaryRange,
            ClosingDate = jobPostingDto.ClosingDate,
            IsActive = jobPostingDto.IsActive,
            CreatedAt = jobPostingDto.CreatedAt,
            UpdatedAt = jobPostingDto.UpdatedAt
        };
        var categories = _categoryService.GetAll();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.JobPosting = jobPosting;
        return View("AddEditJobPosting", jobPosting);
    }

    [HttpPost]
    public async Task<IActionResult> EditJobPosting(JobPostingViewModel jobPostingViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(jobPostingViewModel);
        }
        var jobPosting = new JobPostingDto
        {
            Id = jobPostingViewModel.Id,
            Title = jobPostingViewModel.Title,
            Description = jobPostingViewModel.Description,
            CompanyName = jobPostingViewModel.CompanyName,
            Location = jobPostingViewModel.Location,
            CategoryId = jobPostingViewModel.CategoryId,
            EmploymentType = jobPostingViewModel.EmploymentType,
            SalaryRange = jobPostingViewModel.SalaryRange,
            ClosingDate = jobPostingViewModel.ClosingDate,
            IsActive = jobPostingViewModel.IsActive,
        };
        await _jobService.UpdateAsync(jobPosting);
        return Ok(new { Success = true, Message = "Job posting updated successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> AllJobPostings(JobPostingFilterDto jobPostingFilterDto)
    {
        var jobPostingsDto = await _jobService.GetJobPostingsAsync(jobPostingFilterDto);
        var jobPostings = jobPostingsDto.Items.Select(j => new JobPostingViewModel
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            CompanyName = j.CompanyName,
            Location = j.Location,
            CategoryId = j.CategoryId,
            CategoryName = j.CategoryName,
            EmploymentType = j.EmploymentType,
            SalaryRange = j.SalaryRange,
            ClosingDate = j.ClosingDate,
            IsActive = j.IsActive,
            CreatedAt = j.CreatedAt,
            UpdatedAt = j.UpdatedAt
        }).ToList();
        return View("AllJobPostings", jobPostings);
    }

    [HttpGet]
    public async Task<PaginationResponseDto<JobPostingViewModel>> GetAllJobPostings(JobPostingFilterDto jobPostingFilterDto)
    {
        var jobPostingsDto = await _jobService.GetJobPostingsAsync(jobPostingFilterDto);

        var jobPostings = jobPostingsDto.Items.Select(j => new JobPostingViewModel
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            CompanyName = j.CompanyName,
            Location = j.Location,
            CategoryId = j.CategoryId,
            CategoryName = j.CategoryName,
            EmploymentType = j.EmploymentType,
            SalaryRange = j.SalaryRange,
            ClosingDate = j.ClosingDate,
            IsActive = j.IsActive,
            CreatedAt = j.CreatedAt,
            UpdatedAt = j.UpdatedAt
        }).ToList();

        return new PaginationResponseDto<JobPostingViewModel>
        {
            Items = jobPostings,
            TotalCount = jobPostingsDto.TotalCount,
            Page = jobPostingsDto.Page,
            PageSize = jobPostingsDto.PageSize
        };
    }

    [HttpPost]
    public async Task<IActionResult> DeleteJobPosting(Guid jobId)
    {
        await _jobService.DeleteAsync(jobId);
        return Ok(new { Success = true, Message = "Job posting deleted successfully." });
    }

    [HttpGet]
    public IActionResult JobPostingDetails(Guid jobId)
    {
        var jobPostingDto = _jobService.GetById(jobId);
        var jobPosting = new JobPostingViewModel
        {
            Id = jobPostingDto.Id,
            Title = jobPostingDto.Title,
            Description = jobPostingDto.Description,
            CompanyName = jobPostingDto.CompanyName,
            Location = jobPostingDto.Location,
            CategoryId = jobPostingDto.CategoryId,
            CategoryName = jobPostingDto.CategoryName,
            EmploymentType = jobPostingDto.EmploymentType,
            SalaryRange = jobPostingDto.SalaryRange,
            ClosingDate = jobPostingDto.ClosingDate,
            IsActive = jobPostingDto.IsActive,
            CreatedAt = jobPostingDto.CreatedAt,
            UpdatedAt = jobPostingDto.UpdatedAt
        };
        return View("JobDetails", jobPosting);
    }
}
