using Shared.DTOs;

namespace BLL.Interfaces;

public interface ICandidateService
{
    Task<AdminProfileDto> GetAdminProfile(Guid userId);
    Task<CandidateProfileDto> GetCandidateProfile(Guid candidateId);
    Task<bool> UpdateCandidateProfile(CandidateProfileDto candidateProfile);
    Task<bool> UpdateAdminProfile(AdminProfileDto adminProfile);
    Task<bool> ToggleCandidateStatus(Guid candidateId);
    Task<PaginationResponseDto<CandidateProfileDto>> GetCandidatesAsync(CandidateFilterDto filter);
}
