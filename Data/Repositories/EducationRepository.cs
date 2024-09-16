using Data.Interfaces;
using Data.Models;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class EducationRepository : BaseRepository, IEducationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EducationRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public EducationRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<College> GetCollegesWithIncludes()
        {
            return this.GetDbSet<College>()
                        .Where(c => !c.IsDeleted)
                        .Include(c => c.Departments);
        }

        public async Task<List<College>> GetAllCollegesAsync() =>
            await GetCollegesWithIncludes().AsNoTracking().ToListAsync();

        public async Task<List<Department>> GetAllDepartmentsAsync() =>
            await this.GetDbSet<Department>().Where(d => !d.IsDeleted).AsNoTracking().ToListAsync();

        public async Task<List<YearLevel>> GetAllYearLevelsAsync() =>
            await this.GetDbSet<YearLevel>().AsNoTracking().ToListAsync();

        public async Task AddCollegeAsync(College college)
        {
            await this.GetDbSet<College>().AddAsync(college);
            await this.UnitOfWork.SaveChangesAsync();
        }

        public async Task AddDepartmentAsync(Department department)
        {
            await this.GetDbSet<Department>().AddAsync(department);
            await this.UnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCollegeAsync(College college)
        {
            this.GetDbSet<College>().Update(college);
            await this.UnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            this.GetDbSet<Department>().Update(department);
            await this.UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCollegeAsync(College college)
        {
            college.IsDeleted = true;
            this.GetDbSet<College>().Update(college);
            await this.UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(Department department)
        {
            department.IsDeleted = true;
            this.GetDbSet<Department>().Update(department);
            await this.UnitOfWork.SaveChangesAsync();
        }
    }
}
