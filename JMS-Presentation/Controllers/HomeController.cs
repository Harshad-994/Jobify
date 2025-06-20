using BLL.Interfaces;
using DAL.Data.Enums;
using JMS_Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JMS_Presentation.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDashboardService _dashboardService;
    private readonly ICandidateService _candidateService;
    private readonly IJWTService _jWTService;

    public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService, ICandidateService candidateService, IJWTService jWTService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
        _candidateService = candidateService;
        _jWTService = jWTService;
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
        var user = await _candidateService.GetCandidateProfile(userId);
        var viewModel = new ProfileViewModel
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
        return View("AdminProfile", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _candidateService.GetCandidateProfile(model.UserId);
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        await _candidateService.UpdateCandidateProfile(user);

        var newJwt = _jWTService.GenerateToken(user.Id.ToString(), user.Email, ((Role)user.Role).ToString(), user.FirstName);
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(1)
        };

        Response.Cookies.Append("jwt", newJwt, cookieOptions);

        return Ok(new { Success = true, Message = "Profile updated successfully." });
    }

    // public IActionResult CandidateDashboard()
    // {
    //     return View();
    // }

}
