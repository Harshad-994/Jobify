using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Data.Enums;
using JMS_Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

namespace JMS_Presentation.Controllers;

public class JobCategoryController : Controller
{
    private readonly ICategoryService _categoryService;
    public JobCategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
    public IActionResult AddCategory()
    {
        return View("AddEditCategory", new JobCategoryViewModel { });
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> AddCategory(JobCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var categoryDto = new JobCategoryDto
        {
            Name = model.Name,
            Description = model.Description
        };
        var createdCategory = await _categoryService.CreateAsync(categoryDto);
        TempData["SuccessMessage"] = "Category added successfully";
        return Ok(new SuccessResponce { Success = true, Message = "Category added successfully", RedirectUrl = Url.Action("AllCategories") });
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> EditCategory(Guid categoryId)
    {
        var categoryDto = await _categoryService.GetByIdAsync(categoryId);
        var category = new JobCategoryViewModel
        {
            Id = categoryDto.Id,
            Name = categoryDto.Name,
            Description = categoryDto.Description,
            TotalNoOfJobs = categoryDto.TotalNoOfJobs
        };
        return View("AddEditCategory", category);
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> UpdateCategory(JobCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var categoryDto = new JobCategoryDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
        };
        var updatedCategory = await _categoryService.UpdateAsync(categoryDto);
        TempData["SuccessMessage"] = "Category updated successfully";
        return Ok(new SuccessResponce { Success = true, Message = "Category updated successfully", RedirectUrl = Url.Action("AllCategories") });
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> AllCategories(JobCategoryFilterDto filterDto)
    {
        var categoriesDto = await _categoryService.GetJobCategoriesAsync(filterDto);
        var categories = categoriesDto.Items.Select(c => new JobCategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            TotalNoOfJobs = c.TotalNoOfJobs
        }).ToList();
        return View("AllCategories", categories);
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        await _categoryService.DeleteAsync(categoryId);
        return Ok(new SuccessResponce { Success = true, Message = "Category deleted successfully" });
    }
    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin)+","+nameof(Role.Candidate))]
    public async Task<PaginationResponseDto<JobCategoryViewModel>> GetAllCategories(JobCategoryFilterDto jobCategoryFilter)
    {
        var categoriesDto = await _categoryService.GetJobCategoriesAsync(jobCategoryFilter);
        var categories = categoriesDto.Items.Select(c => new JobCategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            TotalNoOfJobs = c.TotalNoOfJobs
        }).ToList();
        return new PaginationResponseDto<JobCategoryViewModel>
        {
            Items = categories,
            TotalCount = categoriesDto.TotalCount,
            Page = categoriesDto.Page,
            PageSize = categoriesDto.PageSize,
        };
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Candidate))]
    public async Task<IActionResult> AllJobCategories()
    {
        var categoriesDto = await _categoryService.GetJobCategoriesAsync(new JobCategoryFilterDto { });
        var categories = categoriesDto.Items.Select(c => new JobCategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            TotalNoOfJobs = c.TotalNoOfJobs
        }).ToList();
        return View("AllCategoriesForCandidate", categories);
    }
}
