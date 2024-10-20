using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class DashboardViewModel
    {
        #region User related data
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalRecruiters { get; set; }
        public int TotalApplicants { get; set; }
        #endregion

        #region Application related data
        public int TotalApplicationsAllTime { get; set; }
        public int TotalApplicationsThisYear { get; set; }
        public Dictionary<int, int> TotalApplicationsPerMonth { get; set; }
        #endregion

        #region Job related data
        public int TotalJobsAllTime { get; set; }
        public Dictionary<int, int> TotalJobsPerMonth { get; set; }
        public Dictionary<string, int> TotalJobsPerEmploymentType { get; set; }
        public Dictionary<string, int> TotalJobsPerStatusType { get; set; }
        public Dictionary<string, int> TotalJobsPerSetupType { get; set; }
        public Dictionary<string, int> TotalJobsPerProgram { get; set; }
        public Dictionary<string, int> JobSalaryDistribution { get; set; }
        #endregion
    }
}
