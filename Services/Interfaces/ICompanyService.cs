using Data.Models;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICompanyService
    {
        Task<PaginatedList<CompanyViewModel>> GetAllCompaniesAsync(
            string sortBy, string search, bool? verified, bool? hasValidMOA, int pageIndex, int pageSize);
    }
}
