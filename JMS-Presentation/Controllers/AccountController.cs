using System.Security.Claims;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Data.Enums;
using JMS_Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

namespace JMS_Presentation.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IJWTService _jWTService;
    private readonly ICandidateService _candidateService;

    public AccountController(IAccountService accountService, IJWTService jWTService, ICandidateService candidateService)
    {
        _accountService = accountService;
        _jWTService = jWTService;
        _candidateService = candidateService;
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        var jwt = Request.Cookies["Jwt"];
        var refreshToken = Request.Cookies["refreshToken"];
        var role = Role.Unknown.ToString();
        if (jwt != null)
        {
            role = _jWTService.ExtractFromToken(jwt, ClaimTypes.Role);
        }
        else if (refreshToken != null)
        {
            var userId = _jWTService.ExtractFromToken(refreshToken, "UserId");
            var user = await _candidateService.GetCandidateProfile(Guid.Parse(userId));
            role = ((Role)user.Role).ToString();
            var newJwtToBeAdded = _jWTService.GenerateToken(user.Id.ToString(), user.Email, ((Role)user.Role).ToString(), user.FirstName);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1)
            };

            Response.Cookies.Append("jwt", newJwtToBeAdded, cookieOptions);
        }
        return (int)(Role)Enum.Parse(typeof(Role), role) switch
        {
            (int)Role.Admin => RedirectToAction("AdminDashboard", "Home"),
            (int)Role.Candidate => RedirectToAction("CandidateDashboard", "Home"),
            _ => View()
        };
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var loginDto = new LoginDto
        {
            Email = model.Email,
            Password = model.Password
        };
        var user = await _accountService.LoginAsync(loginDto);
        var token = _jWTService.GenerateToken(user.Id.ToString(), user.Email, ((Role)user.Role).ToString(), user.FirstName);
        var refreshToken = _jWTService.GenerateRefreshToken(user.Id.ToString());

        var cookieOptionsForRefreshToken = new CookieOptions
        {
            Expires = model.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(1)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptionsForRefreshToken);
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(1)
        });

        var role = user.Role;

        TempData["SuccessMessage"] = "You have successfully logged in.";
        var redirectUrl = (int)Role.Admin == role ? Url.Action("AdminDashboard", "Home") : Url.Action("CandidateDashboard", "Home");
        return Ok(new SuccessResponce { Success = true, Message = "Login successful.", RedirectUrl = redirectUrl });
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var registerDto = new RegisterDto
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Password = model.Password,
        };
        var user = await _accountService.RegisterAsync(registerDto);
        return Ok(new SuccessResponce { Success = true, Message = "Registration successful.", RedirectUrl = Url.Action("Login", "Account") });
    }

    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        Response.Cookies.Delete("refreshToken");
        TempData["SuccessMessage"] = "You have successfully logged out.";
        return RedirectToAction("Login", "Account");
    }
}
