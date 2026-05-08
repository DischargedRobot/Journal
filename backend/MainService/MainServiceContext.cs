using Microsoft.EntityFrameworkCore;

namespace MainService
{
	public class MainServiceContext : DbContext
	{
		public MainServiceContext() { }

		public MainServiceContext(DbContextOptions<MainServiceContext> options) : base(options) { }

		public DbSet<AcademicYears> AcademicYears => Set<AcademicYears>();
		public DbSet<Brigades> Brigades => Set<Brigades>();
		public DbSet<Departments> Departments => Set<Departments>();
		public DbSet<Disciplines> Disciplines => Set<Disciplines>();
		public DbSet<DisciplinesRegisters> DisciplinesRegisters => Set<DisciplinesRegisters>();
		public DbSet<Faculties> Faculties => Set<Faculties>();
		public DbSet<Groups> Groups => Set<Groups>();
		public DbSet<Lessons> Lessons => Set<Lessons>();
		public DbSet<LessonMarks> LessonMarks => Set<LessonMarks>();
		public DbSet<LessonPresences> LessonPresences => Set<LessonPresences>();
		public DbSet<Marks> Marks => Set<Marks>();
		public DbSet<EmployeePosts> EmployeePosts => Set<EmployeePosts>();
		public DbSet<PresenceStatuses> PresenceStatuses => Set<PresenceStatuses>();
		public DbSet<Professors> Professors => Set<Professors>();
		public DbSet<Semesters> Semesters => Set<Semesters>();
		public DbSet<Students> Students => Set<Students>();
		public DbSet<StudentNotes> StudentNotes => Set<StudentNotes>();
		public DbSet<TrainingDirections> TrainingDirections => Set<TrainingDirections>();
		public DbSet<TypeOfAssessments> TypeOfAssessments => Set<TypeOfAssessments>();
		public DbSet<UniversityEmployers> UniversityEmployers => Set<UniversityEmployers>();
		public DbSet<Users> Users => Set<Users>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

		}

	}
}