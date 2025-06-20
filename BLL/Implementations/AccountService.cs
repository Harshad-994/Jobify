using BLL.Interfaces;
using DAL.Data.Models;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.DTOs;
using Shared.Exceptions;
namespace BLL.Implementations;

public class AccountService : IAccountService
{
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IPasswordEncryptionService passwordEncryptionService, ICandidateRepository candidateRepository, ILogger<AccountService> logger)
    {
        _passwordEncryptionService = passwordEncryptionService;
        _candidateRepository = candidateRepository;
        _logger = logger;
    }

    public async Task<User> RegisterAsync(RegisterDto model)
    {
        if (await _candidateRepository.GetByEmailAsync(model.Email) != null)
        {
            _logger.LogWarning("User with email {Email} already exists.", model.Email);
            throw new EmailAlreadyExistsException(model.Email);
        }
        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Password = await _passwordEncryptionService.EncryptAsync(model.Password),
        };

        await _candidateRepository.AddAsync(user);
        _logger.LogInformation("User with email {Email} registered successfully.", user.Email);
        return user;

    }

    public async Task<User> LoginAsync(LoginDto model)
    {
        var user = await _candidateRepository.GetByEmailAsync(model.Email);
        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found.", model.Email);
            throw new UserNotFoundException(model.Email);
        }

        var inEncryptedPassword = await _passwordEncryptionService.EncryptAsync(model.Password);
        if (user.Password != inEncryptedPassword)
        {
            _logger.LogWarning("Invalid password for user with email {Email}.", user.Email);
            throw new InvalidCurrentPasswordException(model.Email);
        }
        if (!user.IsActive)
        {
            _logger.LogWarning("User with email {Email} is not active.", user.Email);
            throw new UserNotActiveException(model.Email);
        }

        _logger.LogInformation("User with email {Email} logged in successfully.", user.Email);
        return user;
    }

    public async Task<bool> ChangePasswordAsync(Guid UserId, ResetPasswordDto resetPasswordViewModel)
    {
        var user = await _candidateRepository.GetByIdAsync(UserId) ?? throw new Exception("User not found.");
        var inEncryptedPassword = await _passwordEncryptionService.EncryptAsync(resetPasswordViewModel.CurrentPassword);
        if (user.Password != inEncryptedPassword)
        {
            _logger.LogWarning("Invalid current password for user with ID {UserId}.", UserId);
            throw new InvalidCurrentPasswordException(user.Email);
        }
        var inEncryptedNewPassword = await _passwordEncryptionService.EncryptAsync(resetPasswordViewModel.NewPassword);
        user.Password = inEncryptedNewPassword;
        await _candidateRepository.UpdateAsync(user);
        _logger.LogInformation("Password changed successfully for user with ID {UserId}.", UserId);
        return true;
    }

}
