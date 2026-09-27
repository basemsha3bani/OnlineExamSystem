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
        protected  override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
            AppConfiguration configuration = new AppConfiguration();

          
            string conn = configuration.ConnectionString;

            optionsBuilder.UseSqlServer(conn);
         
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {

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
    }

}

