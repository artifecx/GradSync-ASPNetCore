using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class JobViewModel
    {
        [Display(Name = "Job Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Skills Requirement")]
        public List<Skill> Skills { get; set; }

        

        [Display(Name = "Recommended Departments")]
        public List<Department> Departments { get; set; }

        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Date Updated")]
        public DateTime UpdatedDate { get; set; }

        [Display(Name = "Available Slots")]
        public string AvailableSlots { get; set; }

        [Display(Name = "Salary")]
        public string Salary { get; set; }

        public string YearLevelId { get; set; }
        public string CompanyId { get; set; }
        public string RecruiterId { get; set; }
        public string JobId { get; set; }
        public string PostedById { get; set; }
        public string EmploymentTypeId { get; set; }
        public string SetupTypeId { get; set; }
        public string StatusTypeId { get; set; }
        public string ScheduleId { get; set; }

        [Display(Name = "Year Level Requirement")]
        public YearLevel YearLevel { get; set; }

        [Display(Name = "Company")]
        public Company Company { get; set; }

        [Display(Name = "Posted By")]
        public Recruiter PostedBy { get; set; }

        [Display(Name = "Employment Type")]
        public EmploymentType EmploymentType { get; set; }

        [Display(Name = "Status")]
        public StatusType StatusType { get; set; }

        [Display(Name = "Work Setup")]
        public SetupType SetupType { get; set; }

        [Display(Name = "Schedule")]
        public Schedule Schedule { get; set; }
    }
}
