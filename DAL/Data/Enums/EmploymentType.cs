using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DAL.Data.Enums;

public enum EmploymentType
{
    [Display(Name = "Full Time")]
    FullTime = 1,
    [Display(Name = "Part Time")]
    PartTime,
    Contract,
    Internship,
    Freelance,
    Temporary
}
