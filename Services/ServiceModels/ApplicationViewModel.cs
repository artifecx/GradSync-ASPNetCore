using Data.Models;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class ApplicationViewModel
    {
        public string ApplicationId { get; set; }
        public string ApplicationStatusTypeId { get; set; }
        public string UserId { get; set; }
        public string JobId { get; set; }       
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string AdditionalInformationId { get; set; }
        public bool IsArchived { get; set; }
        [Required]
        [Display(Name = "Job Title")]
        public string Title { get; set; }
    }
}
