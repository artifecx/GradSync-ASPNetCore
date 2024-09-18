using System;
using System.Collections.Generic;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class GradSyncDbContext : DbContext
{
    public GradSyncDbContext()
    {
    }

    public GradSyncDbContext(DbContextOptions<GradSyncDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdditionalInformation> AdditionalInformations { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Applicant> Applicants { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ApplicationStatusType> ApplicationStatusTypes { get; set; }

    public virtual DbSet<Avatar> Avatars { get; set; }

    public virtual DbSet<CategoryType> CategoryTypes { get; set; }

    public virtual DbSet<College> Colleges { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyLogo> CompanyLogos { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<EducationalDetail> EducationalDetails { get; set; }

    public virtual DbSet<EmploymentType> EmploymentTypes { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<MemorandumOfAgreement> MemorandumOfAgreements { get; set; }

    public virtual DbSet<Recruiter> Recruiters { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SavedJob> SavedJobs { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<StatusType> StatusTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<YearLevel> YearLevels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdditionalInformation>(entity =>
        {
            entity.ToTable("AdditionalInformation");

            entity.HasIndex(e => e.Type, "IX_AdditionalInformation_Type");

            entity.HasIndex(e => e.UploadedDate, "IX_AdditionalInformation_UploadedDate");

            entity.Property(e => e.AdditionalInformationId).HasMaxLength(256);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("Admin");

            entity.Property(e => e.UserId).HasMaxLength(256);

            entity.HasOne(d => d.User).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_User");
        });

        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("Applicant");

            entity.HasIndex(e => e.EducationalDetailsId, "UQ__Applican__CFAA09A77EF517AE").IsUnique();

            entity.HasIndex(e => e.ResumeId, "UQ__Applican__D7D7A0F65622CE6A").IsUnique();

            entity.Property(e => e.UserId).HasMaxLength(256);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.EducationalDetailsId).HasMaxLength(256);
            entity.Property(e => e.IdNumber).HasMaxLength(256);
            entity.Property(e => e.ResumeId).HasMaxLength(256);

            entity.HasOne(d => d.EducationalDetails).WithOne(p => p.Applicant)
                .HasForeignKey<Applicant>(d => d.EducationalDetailsId)
                .HasConstraintName("FK_Applicant_EducationalDetails");

            entity.HasOne(d => d.Resume).WithOne(p => p.Applicant)
                .HasForeignKey<Applicant>(d => d.ResumeId)
                .HasConstraintName("FK_Applicant_Resume");

            entity.HasOne(d => d.User).WithOne(p => p.Applicant)
                .HasForeignKey<Applicant>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applicant_User");

            entity.HasMany(d => d.Skills).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "ApplicantSkill",
                    r => r.HasOne<Skill>().WithMany()
                        .HasForeignKey("SkillsId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ApplicantSkills_Skills"),
                    l => l.HasOne<Applicant>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ApplicantSkills_Applicant"),
                    j =>
                    {
                        j.HasKey("UserId", "SkillsId");
                        j.ToTable("ApplicantSkills");
                        j.IndexerProperty<string>("UserId").HasMaxLength(256);
                        j.IndexerProperty<string>("SkillsId").HasMaxLength(256);
                    });
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("Application");

            entity.HasIndex(e => e.IsArchived, "IX_Application_IsArchived");

            entity.HasIndex(e => e.JobId, "IX_Application_JobId");

            entity.HasIndex(e => e.UserId, "IX_Application_UserId");

            entity.Property(e => e.ApplicationId).HasMaxLength(256);
            entity.Property(e => e.AdditionalInformationId).HasMaxLength(256);
            entity.Property(e => e.ApplicationStatusTypeId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.JobId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasOne(d => d.ApplicationStatusType).WithMany(p => p.Applications)
                .HasForeignKey(d => d.ApplicationStatusTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_ApplicationStatusType");

            entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_Job");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_User");
        });

        modelBuilder.Entity<ApplicationStatusType>(entity =>
        {
            entity.ToTable("ApplicationStatusType");

            entity.HasIndex(e => e.ApplicationStatusTypeId, "IX_Application_ApplicationStatusTypeId");

            entity.Property(e => e.ApplicationStatusTypeId).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Avatar>(entity =>
        {
            entity.ToTable("Avatar");

            entity.Property(e => e.AvatarId).HasMaxLength(256);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<CategoryType>(entity =>
        {
            entity.ToTable("CategoryType");

            entity.Property(e => e.CategoryTypeId).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<College>(entity =>
        {
            entity.ToTable("College");

            entity.HasIndex(e => e.IsDeleted, "IX_College_IsDeleted");

            entity.HasIndex(e => e.Name, "IX_College_Name");

            entity.HasIndex(e => e.ShortName, "IX_College_ShortName");

            entity.Property(e => e.CollegeId).HasMaxLength(256);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.ShortName)
                .IsRequired()
                .HasMaxLength(256);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company");

            entity.HasIndex(e => e.IsArchived, "IX_Company_IsArchived");

            entity.HasIndex(e => e.IsVerified, "IX_Company_IsVerified");

            entity.HasIndex(e => e.MemorandumOfAgreementId, "IX_Company_MOAId");

            entity.Property(e => e.CompanyId).HasMaxLength(256);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CompanyLogoId).HasMaxLength(256);
            entity.Property(e => e.ContactEmail)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.ContactNumber).HasMaxLength(256);
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.MemorandumOfAgreementId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasOne(d => d.CompanyLogo).WithMany(p => p.Companies)
                .HasForeignKey(d => d.CompanyLogoId)
                .HasConstraintName("FK_Company_CompanyLogo");

            entity.HasOne(d => d.MemorandumOfAgreement).WithMany(p => p.Companies)
                .HasForeignKey(d => d.MemorandumOfAgreementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Company_MOA");
        });

        modelBuilder.Entity<CompanyLogo>(entity =>
        {
            entity.ToTable("CompanyLogo");

            entity.Property(e => e.CompanyLogoId).HasMaxLength(256);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.HasIndex(e => e.CollegeId, "IX_Department_CollegeId");

            entity.HasIndex(e => e.IsDeleted, "IX_Department_IsDeleted");

            entity.HasIndex(e => e.Name, "IX_Department_Name");

            entity.HasIndex(e => e.ShortName, "IX_Department_ShortName");

            entity.Property(e => e.DepartmentId).HasMaxLength(256);
            entity.Property(e => e.CollegeId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.ShortName)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasOne(d => d.College).WithMany(p => p.Departments)
                .HasForeignKey(d => d.CollegeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Department_College");
        });

        modelBuilder.Entity<EducationalDetail>(entity =>
        {
            entity.HasKey(e => e.EducationalDetailsId);

            entity.HasIndex(e => e.IdNumber, "IX_EducationalDetails_IdNumber");

            entity.HasIndex(e => e.IsGraduate, "IX_EducationalDetails_IsGraduate");

            entity.Property(e => e.EducationalDetailsId).HasMaxLength(256);
            entity.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.IdNumber)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.YearLevelId)
                .IsRequired()
                .HasMaxLength(256);
        });

        modelBuilder.Entity<EmploymentType>(entity =>
        {
            entity.ToTable("EmploymentType");

            entity.Property(e => e.EmploymentTypeId).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.ToTable("Job");

            entity.HasIndex(e => e.CategoryTypeId, "IX_Job_CategoryTypeId");

            entity.HasIndex(e => e.DepartmentId, "IX_Job_Department");

            entity.HasIndex(e => e.EmploymentTypeId, "IX_Job_EmploymentTypeId");

            entity.HasIndex(e => e.IsArchived, "IX_Job_IsArchived");

            entity.HasIndex(e => e.Location, "IX_Job_Location");

            entity.HasIndex(e => e.PostedById, "IX_Job_PostedById");

            entity.HasIndex(e => e.Salary, "IX_Job_Salary");

            entity.HasIndex(e => e.ScheduleId, "IX_Job_ScheduleId");

            entity.HasIndex(e => e.StatusTypeId, "IX_Job_StatusTypeId");

            entity.HasIndex(e => e.Title, "IX_Job_Title");

            entity.HasIndex(e => e.YearLevelId, "IX_Job_YearLevelId");

            entity.Property(e => e.JobId).HasMaxLength(256);
            entity.Property(e => e.CategoryTypeId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(800);
            entity.Property(e => e.EmploymentTypeId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.PostedById)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Salary)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.ScheduleId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.StatusTypeId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.YearLevelId)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasOne(d => d.CategoryType).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CategoryTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_CategoryType");

            entity.HasOne(d => d.Department).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_Department");

            entity.HasOne(d => d.EmploymentType).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.EmploymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_EmploymentType");

            entity.HasOne(d => d.PostedBy).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.PostedById)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_User");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_Schedule");

            entity.HasOne(d => d.StatusType).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.StatusTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_StatusType");

            entity.HasOne(d => d.YearLevel).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.YearLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_YearLevel");

            entity.HasMany(d => d.Skills).WithMany(p => p.Jobs)
                .UsingEntity<Dictionary<string, object>>(
                    "JobSkill",
                    r => r.HasOne<Skill>().WithMany()
                        .HasForeignKey("SkillsId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_JobSkills_Skills"),
                    l => l.HasOne<Job>().WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_JobSkills_Job"),
                    j =>
                    {
                        j.HasKey("JobId", "SkillsId");
                        j.ToTable("JobSkills");
                        j.IndexerProperty<string>("JobId").HasMaxLength(256);
                        j.IndexerProperty<string>("SkillsId").HasMaxLength(256);
                    });
        });

        modelBuilder.Entity<MemorandumOfAgreement>(entity =>
        {
            entity.ToTable("MemorandumOfAgreement");

            entity.HasIndex(e => e.ValidityEnd, "IX_MOA_ValidityEnd");

            entity.HasIndex(e => e.ValidityStart, "IX_MOA_ValidityStart");

            entity.Property(e => e.MemorandumOfAgreementId).HasMaxLength(256);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ValidityEnd).HasColumnType("datetime");
            entity.Property(e => e.ValidityStart)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Recruiter>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("Recruiter");

            entity.HasIndex(e => e.CompanyId, "IX_Recruiter_CompanyId");

            entity.HasIndex(e => e.CompanyId, "UQ__Recruite__2D971CAD11E7CB6A").IsUnique();

            entity.Property(e => e.UserId).HasMaxLength(256);
            entity.Property(e => e.CompanyId).HasMaxLength(256);
            entity.Property(e => e.Title).HasMaxLength(256);

            entity.HasOne(d => d.Company).WithOne(p => p.Recruiter)
                .HasForeignKey<Recruiter>(d => d.CompanyId)
                .HasConstraintName("FK_Recruiter_Company");

            entity.HasOne(d => d.User).WithOne(p => p.Recruiter)
                .HasForeignKey<Recruiter>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recruiter_User");
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.ToTable("Resume");

            entity.HasIndex(e => e.UploadedDate, "IX_Resume_UploadedDate");

            entity.Property(e => e.ResumeId).HasMaxLength(256);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "IX_Role_Name");

            entity.Property(e => e.RoleId).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<SavedJob>(entity =>
        {
            entity.ToTable("SavedJob");

            entity.Property(e => e.SavedJobId).HasMaxLength(256);
            entity.Property(e => e.JobId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.SaveDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasOne(d => d.Job).WithMany(p => p.SavedJobs)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavedJob_Job");

            entity.HasOne(d => d.User).WithMany(p => p.SavedJobs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavedJob_User");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("Schedule");

            entity.HasIndex(e => e.Days, "IX_Schedule_Days");

            entity.HasIndex(e => e.Hours, "IX_Schedule_Hours");

            entity.Property(e => e.ScheduleId).HasMaxLength(256);
            entity.Property(e => e.Days)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Hours).HasMaxLength(256);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillsId);

            entity.HasIndex(e => e.Name, "IX_Skills_Name");

            entity.HasIndex(e => e.Type, "IX_Skills_Type");

            entity.Property(e => e.SkillsId).HasMaxLength(256);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(256);
        });

        modelBuilder.Entity<StatusType>(entity =>
        {
            entity.ToTable("StatusType");

            entity.Property(e => e.StatusTypeId).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.AvatarId, "IX_User_AvatarId");

            entity.HasIndex(e => e.Email, "IX_User_Email");

            entity.HasIndex(e => e.FirstName, "IX_User_FirstName");

            entity.HasIndex(e => e.IsDeleted, "IX_User_IsDeleted");

            entity.HasIndex(e => e.IsVerified, "IX_User_IsVerified");

            entity.HasIndex(e => e.LastName, "IX_User_LastName");

            entity.HasIndex(e => e.RoleId, "IX_User_RoleId");

            entity.HasIndex(e => e.AvatarId, "UQ__User__4811D66B40514DEC").IsUnique();

            entity.Property(e => e.UserId).HasMaxLength(256);
            entity.Property(e => e.AvatarId).HasMaxLength(256);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.JoinDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.RoleId)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Suffix).HasMaxLength(100);

            entity.HasOne(d => d.Avatar).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.AvatarId)
                .HasConstraintName("FK_User_Avatar");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<YearLevel>(entity =>
        {
            entity.ToTable("YearLevel");

            entity.HasIndex(e => e.Name, "IX_YearLevel_Name");

            entity.HasIndex(e => e.Year, "IX_YearLevel_Year");

            entity.Property(e => e.YearLevelId).HasMaxLength(256);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(256);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
