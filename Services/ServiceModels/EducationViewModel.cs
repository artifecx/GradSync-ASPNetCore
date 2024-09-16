using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class EducationViewModel
    {
        public string CollegeName { get; set; }
        public string CollegeShortName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentShortName { get; set; }

        public List<College> Colleges { get; set; }
        public List<Department> Departments { get; set; }
        public List<YearLevel> YearLevels { get; set; }
    }
}
