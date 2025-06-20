using BLL.Interfaces;
using DAL.Data.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.DTOs;
using Shared.Exceptions;

namespace BLL.Implementations;

public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly IJobApplicationService _jobApplicationService;
    private readonly ILogger<CandidateService> _logger;
    public CandidateService(ILogger<CandidateService> logger, ICandidateRepository candidateRepository, IJobApplicationService jobApplicationService)
    {
        _logger = logger;
        _candidateRepository = candidateRepository;
        _jobApplicationService = jobApplicationService;
    }

    public async Task<CandidateProfileDto> GetCandidateProfile(Guid candidateId)
    {
        var candidateEntity = await _candidateRepository.GetByIdAsync(candidateId);
        if (candidateEntity == null)
        {
            _logger.LogWarning("Candidate with id {candidateId} not found.", candidateId);
            throw new CandidateNotFoundException(candidateId);
        }
        var candidateProfileDto = new CandidateProfileDto
        {

            Id = candidateEntity.Id,
            FirstName = candidateEntity.FirstName,
            LastName = candidateEntity.LastName,
            Email = candidateEntity.Email,
            Role = candidateEntity.Role,
            IsActive = candidateEntity.IsActive,
            ResumeUrl = candidateEntity.ResumeUrl,
            UpdatedAt = candidateEntity.UpdatedAt,
            JobApplications = _jobApplicationService.GetAllApplicationsOfCandidateAsync(candidateEntity.Id).Result

        };
        return candidateProfileDto;
    }

    public async Task<bool> UpdateCandidateProfile(CandidateProfileDto candidateProfile)
    {
        var candidateEntity = await _candidateRepository.GetByIdAsync(candidateProfile.Id);

        if (candidateEntity == null)
        {
            _logger.LogWarning("Candidate with id {candidateId} not found.", candidateProfile.Id);
            throw new CandidateNotFoundException(candidateProfile.Id);
        }

        candidateEntity.FirstName = candidateProfile.FirstName;
        candidateEntity.LastName = candidateProfile.LastName;
        if (candidateEntity.Role == (int)Role.Candidate)
        {
            candidateEntity.ResumeUrl = candidateProfile.ResumeUrl;
        }
        candidateEntity.UpdatedAt = DateTime.UtcNow;
        await _candidateRepository.UpdateAsync(candidateEntity);
        _logger.LogInformation("Candidate profile with id {candidateId} updated successfully", candidateProfile.Id);
        return true;
    }

    public async Task<bool> ToggleCandidateStatus(Guid candidateId)
    {
        var candidateEntity = await _candidateRepository.GetByIdAsync(candidateId);

        if (candidateEntity == null)
        {
            _logger.LogWarning("Candidate with id {candidateId} not found.", candidateId);
            throw new CandidateNotFoundException(candidateId);
        }

        candidateEntity.IsActive = !candidateEntity.IsActive;
        await _candidateRepository.UpdateAsync(candidateEntity);
        _logger.LogInformation("Candidate status with id {candidateId} toggled successfully", candidateId);
        return true;
    }

    public async Task<PaginationResponseDto<CandidateProfileDto>> GetCandidatesAsync(CandidateFilterDto filter)
    {
        var query = _candidateRepository.GetAll();
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var term = filter.SearchText.ToLower().Trim();
            query = query.Where(c =>
            c.FirstName.ToLower().Contains(term) ||
            c.LastName.ToLower().Contains(term) ||
            c.Email.ToLower().Contains(term) ||
            (c.FirstName.ToLower() + " " + c.LastName.ToLower()).Contains(term)
            );
        }


        var items = query
            .OrderBy(c => c.Id)
            .Where(c => c.Role == (int)Role.Candidate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(c => new CandidateProfileDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Role = c.Role,
                IsActive = c.IsActive,
                ResumeUrl = c.ResumeUrl,
                UpdatedAt = c.UpdatedAt,
                CreatedAt = c.CreatedAt
            });

        var totalCount = await items.CountAsync();

        return new PaginationResponseDto<CandidateProfileDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

}
