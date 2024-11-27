using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ReferenceDataRepository : BaseRepository, IReferenceDataRepository
    {
        public ReferenceDataRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<List<EmploymentType>> GetEmploymentTypesAsync() =>
            await this.GetDbSet<EmploymentType>().AsNoTracking().ToListAsync();

        public async Task<List<StatusType>> GetStatusTypesAsync() =>
            await this.GetDbSet<StatusType>().Where(s => s.StatusTypeId != "Pending").AsNoTracking().ToListAsync();

        public async Task<List<SetupType>> GetWorkSetupsAsync() =>
            await this.GetDbSet<SetupType>().AsNoTracking().ToListAsync();

        public async Task<List<YearLevel>> GetYearLevelsAsync() =>
            await this.GetDbSet<YearLevel>()
                .OrderByDescending(y => y.Year)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<ApplicationStatusType>> GetApplicationStatusTypesAsync() =>
            await this.GetDbSet<ApplicationStatusType>()
                .Where(s => s.ApplicationStatusTypeId != "Viewed" 
                    && s.ApplicationStatusTypeId != "Submitted")
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Role>> GetUserRolesAsync() =>
            await this.GetDbSet<Role>().AsNoTracking().ToListAsync();

        public async Task<List<Program>> GetProgramsAsync() =>
            await this.GetDbSet<Program>()
                .Where(p => !p.IsDeleted)
                .Include(p => p.Department)
                .ThenInclude(d => d.College)
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Department>> GetDepartmentsAsync() =>
            await this.GetDbSet<Department>()
                .Where(p => !p.IsDeleted)
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<College>> GetCollegesAsync() =>
            await this.GetDbSet<College>()
                .Where(p => !p.IsDeleted)
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Skill>> GetSkillsAsync() =>
            await this.GetDbSet<Skill>()
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Skill>> GetSoftSkillsAsync() =>
            await this.GetDbSet<Skill>()
                .Where(s => s.Type == "Common Skill")
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        public async Task<List<Skill>> GetTechnicalSkillsAsync() =>
            await this.GetDbSet<Skill>()
                .Where(s => s.Type == "Specialized Skill")
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        public async Task<List<Skill>> GetCertificationSkillsAsync() =>
            await this.GetDbSet<Skill>()
                .Where(s => s.Type == "Certification")
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
    }
}
