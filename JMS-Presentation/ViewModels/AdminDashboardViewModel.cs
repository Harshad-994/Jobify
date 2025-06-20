namespace JMS_Presentation.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalNoOfJobs { get; set; }
    public int TotalNoOfCandidates { get; set; }
    public int TotalNoOfApplications { get; set; }
    public List<JobPostingViewModel> RecentJobPostings { get; set; } = new List<JobPostingViewModel>();
}
