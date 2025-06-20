using System.ComponentModel.DataAnnotations;

namespace DAL.Data.Enums;

public enum ApplicationStatus
{
    Applied = 1,
    [Display(Name = "Under Review")]
    UnderReview,
    [Display(Name = "Interview Scheduled")]
    InterviewScheduled,
    Offered,
    Rejected,
    Withdrawn
}