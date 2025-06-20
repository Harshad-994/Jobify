using Shared.DTOs;
using DAL.Data.Models;

namespace BLL.Interfaces;

public interface IAccountService
{
    Task<User> RegisterAsync(RegisterDto model);
    Task<User> LoginAsync(LoginDto model);
    Task<bool> ChangePasswordAsync(Guid UserId, ResetPasswordDto resetPasswordViewModel);
}