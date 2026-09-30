using DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DataRepository.GateWay
{
    public class DbConext:DbContext
    {
        
       public DbConext()
        {
           

           

            
        }
        public DbConext(DbContextOptions<DbConext> options) : base(options) { }

        protected  override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
           
            AppConfiguration configuration = new AppConfiguration();

          
            string conn = configuration.ConnectionString;

            optionsBuilder.UseSqlServer(conn);
         
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            modelBuilder.Entity<ExaminerAttempt>().Property(a => a.Score).HasPrecision(18, 10);
            modelBuilder.Entity<ExaminerAttempt>().Property(a => a.Status).HasMaxLength(20);
            modelBuilder.Entity<ExaminerAttempt>().HasIndex(a => new { a.UserId, a.StartedAt });
            modelBuilder.Entity<ExaminerAttempt>().HasIndex(a => a.Status);

            modelBuilder.Entity<QuestionAnswers>().HasKey(o => o.Id);
            modelBuilder.Entity<ExamTypesDetails>().HasKey(o => o.Id);
            modelBuilder.Entity<ExamTypes>().HasKey(o => o.Id);
            modelBuilder.Entity<DifficultyLevels>().HasKey(o => o.Id);
            modelBuilder.Entity<User>().HasKey(o => o.Id);
        }




        public DbSet<Questions> Questions { get; set; }

        public  DbSet<QuestionAnswers> QuestionAnswers { get; set; }


        public DbSet<Exams> Exams { get; set; }

        public DbSet<ExamSectionRules> ExamSectionRules { get; set; }

        public DbSet<ExamSections> ExamSections { get; set; }

        public DbSet<DifficultyLevels> DifficultyLevels { get; set; }

        public DbSet<StudySubject> StudySubjects { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<ExaminerAttempt> ExaminerAttempts { get; set; }
    }

}

