using Data.Models;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace Services.ServiceModels
{
    public class ApplicationViewModel
    {
        #region Applicant Information
        public string ApplicantId { get; set; }

        [Display(Name = "Applicant")]
        public Applicant Applicant { get; set; }

        public string ApplicantName { get; set; }
        #endregion

        #region Job Information
        [Display(Name = "Job")]
        public Job Job { get; set; }

        public string JobId { get; set; }

        public string RecruiterName { get; set; }
        #endregion

        #region Application Information 
        public string ApplicationId { get; set; }

        public string ApplicationStatusTypeId { get; set; }

        [Display(Name = "Status")]
        public ApplicationStatusType ApplicationStatusType { get; set; }

        [Display(Name = "Application Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Updated")]
        public DateTime UpdatedDate { get; set; }
        #endregion
    }
}
