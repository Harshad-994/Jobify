using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Data.Enums;
using DAL.Data.Models;
using JMS_Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.DTOs;
using Shared.Models;

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
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> AddJobPosting()
    {
        var categories = await _categoryService.GetJobCategoriesAsync(new JobCategoryFilterDto() { PageSize = 0 });
        ViewBag.Categories = new SelectList(categories.Items, "Id", "Name");
        return View("AddEditJobPosting", new JobPostingViewModel { });
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Admin))]
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
        TempData["SuccessMessage"] = "Job posting created successfully.";
        return Ok(new SuccessResponce { Success = true, Message = "Job posting created successfully.", RedirectUrl = Url.Action("AllJobPostings") });
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> EditJobPosting(Guid jobId)
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
        var categories = await _categoryService.GetJobCategoriesAsync(new JobCategoryFilterDto() { PageSize = 0 });
        ViewBag.Categories = new SelectList(categories.Items, "Id", "Name");
        ViewBag.JobPosting = jobPosting;
        return View("AddEditJobPosting", jobPosting);
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Admin))]
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
        TempData["SuccessMessage"] = "Job posting updated successfully.";
        return Ok(new SuccessResponce { Success = true, Message = "Job posting updated successfully.", RedirectUrl = Url.Action("AllJobPostings") });
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
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
    [Authorize(Roles = nameof(Role.Admin) + "," + nameof(Role.Candidate))]
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
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> DeleteJobPosting(Guid jobId)
    {
        await _jobService.DeleteAsync(jobId);
        return Ok(new SuccessResponce { Success = true, Message = "Job posting deleted successfully." });
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin) + "," + nameof(Role.Candidate))]
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

    [HttpGet]
    [Authorize(Roles = nameof(Role.Candidate))]
    public async Task<IActionResult> JobPostings(JobPostingFilterDto jobPostingFilterDto)
    {
        jobPostingFilterDto.IsActive = true;
        jobPostingFilterDto.ClosingDateEnd = DateTime.UtcNow;
        ViewBag.CategoryId = jobPostingFilterDto.CategoryId;

        var categories = await _categoryService.GetJobCategoriesAsync(new JobCategoryFilterDto() { PageSize = 0 });
        ViewBag.Categories = new SelectList(categories.Items, "Id", "Name");
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
        return View("AllJobPostingsForCandidate", jobPostings);
    }
}