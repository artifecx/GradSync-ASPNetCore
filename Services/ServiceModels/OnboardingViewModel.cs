using Data.Models;
using Microsoft.AspNetCore.Http;
using Services.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class OnboardingViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }

        #region Educational Details
        [Required(ErrorMessage = "ID Number is required.")]
        [Display(Name = "ID Number")]
        public string IdNumber { get; set; }

        [Required(ErrorMessage = "Year Level is required.")]
        [Display(Name = "Year Level")]
        public string YearLevelId { get; set; }

        [Required(ErrorMessage = "Program is required.")]
        [Display(Name = "Program")]
        public string ProgramId { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }

        [Required(ErrorMessage = "College is required.")]
        [Display(Name = "College")]
        public string CollegeId { get; set; }
        #endregion

        #region Resume & Skills
        [Required(ErrorMessage = "Resume is required.")]
        [FileValidation([".pdf"], 5)]
        public IFormFile Resume { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Cultural & Soft Skills (Required)")]
        public List<Skill> SkillsS { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Technical Skills (Required)")]
        public List<Skill> SkillsT { get; set; }

        [Display(Name = "Certifications (Optional)")]
        public List<Skill> SkillsC { get; set; }
        #endregion
    }
}
