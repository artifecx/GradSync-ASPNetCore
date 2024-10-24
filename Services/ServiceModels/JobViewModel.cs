using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class JobViewModel
    {
        public string JobId { get; set; }

        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Date Updated")]
        public DateTime UpdatedDate { get; set; }
        public string CompanyId { get; set; }
        public string RecruiterId { get; set; }

        public string StatusTypeId { get; set; }
        
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



        [Required]
        [Display(Name = "Job Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Year Level Requirement")]
        public string YearLevelId { get; set; }

        [Required]
        [Display(Name = "Setup Type")]
        public string SetupTypeId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Available slots must be greater than zero.")]
        [Display(Name = "Available Slots")]
        public int? AvailableSlots { get; set; }

        [Display(Name = "Salary Range (Monthly)")]
        public string Salary { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Lower bound must be a non-negative number.")]
        [Display(Name = "Lower")]
        public double? SalaryLower { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Upper bound must be a non-negative number.")]
        [Display(Name = "Upper")]
        public double? SalaryUpper { get; set; }

        [Display(Name = "Schedule (Weekly)")]
        public string Schedule { get; set; }
        [Required]
        [Range(0, 7, ErrorMessage = "Days must be between 0 (flexible) and 7.")]
        [Display(Name = "Days")]
        public int? ScheduleDays { get; set; }
        [Required]
        [Range(0, 60, ErrorMessage = "Hours must be between 0 (flexible) and 60.")]
        [Display(Name = "Hours")]
        public int? ScheduleHours { get; set; }

        [Required]
        [Display(Name = "Employment Type")]
        public string EmploymentTypeId { get; set; }

        [Required]
        [Display(Name = "Skills Requirement")]
        public List<Skill> Skills { get; set; }

        [Required]
        [Display(Name = "Recommended Programs")]
        public List<Program> Programs { get; set; }

        public string PostedById { get; set; }

        public bool HasApplied { get; set; } = false;
    }
}
