using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IEducationRepository
    {
        Task<List<College>> GetAllCollegesAsync();
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<List<YearLevel>> GetAllYearLevelsAsync();
        Task AddCollegeAsync(College college);
        Task AddDepartmentAsync(Department department);
        Task UpdateCollegeAsync(College college);
        Task UpdateDepartmentAsync(Department department);
        Task DeleteCollegeAsync(College college);
        Task DeleteDepartmentAsync(Department department);
    }
}
