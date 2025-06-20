using DAL.Data.Models;
using DAL.Interfaces;
using Shared.DTOs;

namespace BLL.Interfaces;

public interface IJobApplicationService
{
    Task<bool> ApplyAsync(JobApplicationDto jobApplication);
    Task<List<JobApplicationDto>> GetAllApplicationsOfCandidateAsync(Guid candidateId);
    Task<bool> UpdateApplicationStatusAsync(Guid applicationId, int newStatus);
    Task<bool> DeleteApplicationAsync(Guid applicationId);
    Task<PaginationResponseDto<JobApplicationDto>> GetJobApplicationsAsync(JobApplicationFilterDto filter);
    Task<JobApplicationDto> GetApplicationByIdAsync(Guid applicationId);
}
