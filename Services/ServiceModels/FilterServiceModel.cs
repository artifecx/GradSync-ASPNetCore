using Data.Models;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace Services.ServiceModels
{
    public class FilterServiceModel
    {
        public string UserId { get; set; }
        public string UserRole { get; set; }

        public string Search { get; set; } = "";
        public string SortBy { get; set; } = "";

        public string Role { get; set; } = "";

        public bool? Verified { get; set; } = null;
        public bool? HasValidMOA { get; set; } = null;

        public string ProgramFilter { get; set; } = "";
        public string WorkSetupFilter { get; set; } = "";
        public string StatusFilter { get; set; } = "";

        public string FilterByCompany { get; set; } = null;
        public List<string> FilterByEmploymentType { get; set; } = new List<string>();
        public string FilterByStatusType { get; set; } = null;
        public List<string> FilterByWorkSetup { get; set; } = new List<string>();
        public string FilterByDatePosted { get; set; } = null;
        public string FilterBySalary { get; set; } = null;

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
