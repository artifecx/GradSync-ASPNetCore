using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class JobMatchRepository : BaseRepository, IJobMatchRepository
    {
        public JobMatchRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task AddJobApplicantMatchesAsync(List<JobApplicantMatch> matches)
        {
            await this.GetDbSet<JobApplicantMatch>().AddRangeAsync(matches);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<object> GetApplicantDetailsByIdAsync(string id) =>
            await this.GetDbSet<Applicant>()
                .Include(a => a.Resume)
                .Include(a => a.ApplicantSkills)
                    .ThenInclude(s => s.Skill)
                .Where(a => a.UserId == id && a.Resume != null && a.ApplicantSkills.Any())
                .Select(a => new
                {
                    resume_text = a.Resume.ExtractedText,
                    technical_skills = a.ApplicantSkills
                        .Where(s => s.Type == "Technical" || s.Type == "Certification")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    cultural_skills = a.ApplicantSkills
                        .Where(s => s.Type == "Cultural")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<List<object>> GetAllApplicantDetailsAsync() =>
            (await this.GetDbSet<Applicant>()
                .Include(a => a.Resume)
                .Include(a => a.ApplicantSkills)
                    .ThenInclude(s => s.Skill)
                .Where(a => a.Resume != null && a.ApplicantSkills.Any())
                .Select(a => new
                {
                    applicant_id = a.UserId,
                    resume_text = a.Resume.ExtractedText,
                    technical_skills = a.ApplicantSkills
                        .Where(s => s.Type == "Technical" || s.Type == "Certification")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    cultural_skills = a.ApplicantSkills
                        .Where(s => s.Type == "Cultural")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                })
                .AsNoTracking()
                .ToListAsync()).Cast<object>().ToList();

        public async Task<object> GetJobDetailsByIdAsync(string id) =>
           await this.GetDbSet<Job>()
               .Include(j => j.JobSkills)
                   .ThenInclude(js => js.Skill)
               .Where(j => !j.IsArchived && j.JobId == id && j.SkillWeights != null && j.JobSkills.Any())
               .Select(j => new
               {
                   title = j.Title,
                   description = j.Description,
                   technical_skills = j.JobSkills
                        .Where(s => s.Type == "Technical" || s.Type == "Certification")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                   cultural_skills = j.JobSkills
                        .Where(s => s.Type == "Cultural")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                   tech_cult_weight = j.SkillWeights
               })
               .AsNoTracking()
               .FirstOrDefaultAsync();

        public async Task<List<object>> GetAllJobDetailsAsync() =>
            (await this.GetDbSet<Job>()
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Where(j => !j.IsArchived && j.SkillWeights != null && j.JobSkills.Any())
                .Select(j => new
                {
                    job_id = j.JobId,
                    title = j.Title,
                    description = j.Description,
                    technical_skills = j.JobSkills
                        .Where(s => s.Type == "Technical" || s.Type == "Certification")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    cultural_skills = j.JobSkills
                        .Where(s => s.Type == "Cultural")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    tech_cult_weight = j.SkillWeights
                })
                .AsNoTracking()
                .ToListAsync()).Cast<object>().ToList();
    }
}
