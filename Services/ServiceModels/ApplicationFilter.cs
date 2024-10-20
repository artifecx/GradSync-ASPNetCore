using Data.Models;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace Services.ServiceModels
{
    public class ApplicationFilter
    {
        public string UserId { get; set; }
        public string UserRole { get; set; }

        public string Search { get; set; } = "";
        public string SortBy { get; set; } = "created_desc";

        public string ProgramFilter { get; set; } = "";
        public string WorkSetupFilter { get; set; } = "";
        public string StatusFilter { get; set; } = "";

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
