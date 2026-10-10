using Microsoft.EntityFrameworkCore;
using University_System.Models;

namespace University_System.Data
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=ESRAA-SHIREF\\SQLEXPRESS;Database=UniversityDB;Trusted_Connection=True;TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseResult> CourseResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Avoid SQL Server "multiple cascade paths" errors
            foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Database-level validation: the last line of defence behind the UI checks
            modelBuilder.Entity<Course>().ToTable(t =>
                t.HasCheckConstraint("CK_Courses_MinDegree_LessOrEqual_Degree", "[MinDegree] <= [Degree]"));
            modelBuilder.Entity<Trainee>().ToTable(t =>
                t.HasCheckConstraint("CK_Trainees_Grade_Range", "[Grade] BETWEEN 0 AND 100"));
            modelBuilder.Entity<Instructor>().ToTable(t =>
                t.HasCheckConstraint("CK_Instructors_Salary_NotNegative", "[Salary] >= 0"));
            modelBuilder.Entity<CourseResult>().ToTable(t =>
                t.HasCheckConstraint("CK_CourseResults_Degree_NotNegative", "[Degree] >= 0"));
        }
    }
}
