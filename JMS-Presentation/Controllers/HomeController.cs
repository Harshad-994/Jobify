using BLL.Interfaces;
using DAL.Data.Enums;
using JMS_Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace JMS_Presentation.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDashboardService _dashboardService;
    private readonly ICandidateService _candidateService;
    private readonly IJWTService _jWTService;
    private readonly ICloudinaryService _cloudinaryService;

    public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService, ICandidateService candidateService, IJWTService jWTService, ICloudinaryService cloudinaryService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
        _candidateService = candidateService;
        _jWTService = jWTService;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    public async Task<IActionResult> AdminDashboardAsync()
    {
        var dashboardStats = await _dashboardService.GetAdminDashboardStats();
        var viewModel = new AdminDashboardViewModel
        {
            TotalNoOfJobs = dashboardStats.TotalNoOfJobs,
            TotalNoOfCandidates = dashboardStats.TotalNoOfCandidates,
            TotalNoOfApplications = dashboardStats.TotalNoOfApplications,
            RecentJobPostings = dashboardStats.RecentJobPostings.Select(j => new JobPostingViewModel
            {
                Id = j.Id,
                Title = j.Title ?? "No Title",
                CategoryName = j.CategoryName,
                CategoryId = j.CategoryId,
                TotalNoOfApplications = j.TotalNoOfApplications
            }).ToList()
        };
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> AdminProfile(Guid userId)
    {
        var user = await _candidateService.GetAdminProfile(userId);
        var viewModel = new AdminProfileViewModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
        return View("AdminProfile", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAdminProfile(AdminProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("AdminProfile", model);
        }
        var user = await _candidateService.GetAdminProfile(model.Id);
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        await _candidateService.UpdateAdminProfile(user);

        var newJwt = _jWTService.GenerateToken(user.Id.ToString(), user.Email, ((Role)user.Role).ToString(), user.FirstName);
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(1)
        };

        Response.Cookies.Append("jwt", newJwt, cookieOptions);

        return Ok(new { Success = true, Message = "Profile updated successfully." });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCandidateProfile(CandidateProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new CandidateProfileDto
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            ResumeUrl = model.ResumeUrl,
            Resume = model.Resume,
            Email = model.Email
        };
        await _candidateService.UpdateCandidateProfile(user);

        var newJwt = _jWTService.GenerateToken(user.Id.ToString(), user.Email, Role.Candidate.ToString(), user.FirstName);
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(1)
        };

        Response.Cookies.Append("jwt", newJwt, cookieOptions);
        return Ok(new { Success = true, Message = "Profile updated successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> CandidateDashboard()
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (userId == null)
        {
            return RedirectToAction("Login");
        }

        var candidateDashboardStatsDto = await _dashboardService.GetCandidateDashboardStats(Guid.Parse(userId));
        var viewModel = new CandidateDashboardViewModel
        {
            TotalNoOfActiveJobs = candidateDashboardStatsDto.TotalNoOfActiveJobs,
            TotalNoOfApplications = candidateDashboardStatsDto.TotalNoOfApplications,
        };
        return View("CandidateDashboard", viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CandidateProfile(Guid userId)
    {
        var user = await _candidateService.GetCandidateProfile(userId);
        var viewModel = new CandidateProfileViewModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ResumeUrl = user.ResumeUrl,
            Role = user.Role
        };
        return View("CandidateProfile", viewModel);
    }

}
