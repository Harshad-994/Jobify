namespace Shared.DTOs;

public class AdminDashboardStatsDto
{
    public int TotalNoOfJobs { get; set; }
    public int TotalNoOfCandidates { get; set; }
    public int TotalNoOfApplications { get; set; }
    public List<JobPostingDto> RecentJobPostings { get; set; } = new List<JobPostingDto>();

}
