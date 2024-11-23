using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class FeaturedJobsViewModel
    {
        public string JobId { get; set; }
        public string Title { get; set; }
        public string Salary { get; set; }
        public List<Skill> Skills { get; set; }
        public string EmploymentTypeName { get; set; }
        public string SetupTypeName { get; set; }
        public string Location { get; set; }
        public string CompanyName { get; set; }
        public string MatchPercentage { get; set; }
    }
}
