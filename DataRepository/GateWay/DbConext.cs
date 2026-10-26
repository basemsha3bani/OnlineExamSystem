using Domain.Entities;
using Domain.Aggregates;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;


namespace DataRepository.GateWay
{
    public class DbConext:DbContext
    {
        DataBaseConnectionConfiguration _connectionConfiguration;
     
        public DbConext(DbContextOptions<DbConext> options, DataBaseConnectionConfiguration connectionConfiguration) : base(options) {
            _connectionConfiguration = connectionConfiguration;
        }

        protected  override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
           
         

          
            string conn = _connectionConfiguration.GetConnectionString();

            optionsBuilder.UseSqlServer(conn);
         
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            modelBuilder.Entity<Exam>().ToTable("Exams");
            modelBuilder.Entity<StudySubject>().ToTable("StudySubjects");
            modelBuilder.Entity<ExamAttempt>().ToTable("ExaminerAttempts");
            modelBuilder.Entity<ExamSections>().Property(s => s.ChapterNumbers).IsRequired();
            modelBuilder.Entity<ExamAttempt>().Property(a => a.Version).IsRowVersion();
            modelBuilder.Entity<ExamSectionRules>().HasOne(r => r.section).WithMany(s => s.examSectionRules).HasForeignKey(r => r.SectionId);
            modelBuilder.Entity<ExamSectionRules>().HasOne(r => r.difficultyLevel).WithMany().HasForeignKey(r => r.DifficultyLevelId);
            modelBuilder.Entity<ExamSections>().HasOne(s => s.Exam).WithMany(e => e.Sections).HasForeignKey(s => s.ExamId);
            modelBuilder.Entity<QuestionAnswers>().HasOne(a => a.Question).WithMany(q => q.QuestionAnswers).HasForeignKey(a => a.QuestionId);
            modelBuilder.Entity<ExaminerPerformance>().HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId);
            modelBuilder.Entity<Exam>().Property(e => e.TotalMarks).HasPrecision(18, 2);
            modelBuilder.Entity<ExamAttempt>().Property(a => a.Score).HasPrecision(18, 10);
            modelBuilder.Entity<ExamAttempt>().Property(a => a.Status).HasMaxLength(20);
            modelBuilder.Entity<ExamAttempt>().HasIndex(a => new { a.UserId, a.StartedAt });
            modelBuilder.Entity<ExamAttempt>().HasIndex(a => a.Status);

            modelBuilder.Entity<QuestionAnswers>().HasKey(o => o.Id);
            modelBuilder.Entity<ExamTypesDetails>().HasKey(o => o.Id);
            modelBuilder.Entity<ExamTypes>().HasKey(o => o.Id);
            modelBuilder.Entity<DifficultyLevels>().HasKey(o => o.Id);
            modelBuilder.Entity<ExaminerPerformance>().HasKey(o => o.UserId);
            modelBuilder.Entity<User>().HasKey(o => o.Id);
        }




        public DbSet<Questions> Questions { get; set; }

        public  DbSet<QuestionAnswers> QuestionAnswers { get; set; }


        public DbSet<Exam> Exams { get; set; }

        public DbSet<ExamSectionRules> ExamSectionRules { get; set; }

        public DbSet<ExamSections> ExamSections { get; set; }

        public DbSet<DifficultyLevels> DifficultyLevels { get; set; }

        public DbSet<StudySubject> StudySubjects { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<ExamAttempt> ExaminerAttempts { get; set; }

        public DbSet<ExaminerPerformance> ExaminerPerformance { get; set; }

        
    }

}

