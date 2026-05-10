using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace MainService
{
	public class MainServiceContext : DbContext
	{
		public MainServiceContext() { }

		public MainServiceContext(DbContextOptions<MainServiceContext> options) : base(options) { }

		public DbSet<AcademicYears> AcademicYears => Set<AcademicYears>();
		public DbSet<Attestations> Attestations => Set<Attestations>();
		public DbSet<AttestationMarks> AttestationMarks => Set<AttestationMarks>();
		public DbSet<AttestationTypes> AttestationTypes => Set<AttestationTypes>();
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
		public DbSet<NotesAboutStudent> StudentNotes => Set<NotesAboutStudent>();
		public DbSet<TrainingDirections> TrainingDirections => Set<TrainingDirections>();
		public DbSet<MarkTypes> MarkTypes => Set<MarkTypes>();
		public DbSet<UniversityEmployers> UniversityEmployers => Set<UniversityEmployers>();
		public DbSet<Users> Users => Set<Users>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Настройка связи многие-ко-многим между Disciplines и Groups
			//  (чтобы было читаемое название таблицы)
			modelBuilder.Entity<Disciplines>()
				.HasMany(discipline => discipline.Groups)
				.WithMany(group => group.Disciplines)
				.UsingEntity<Dictionary<string, object>>(
					right => right
						.HasOne<Groups>()
						.WithMany()
						.HasForeignKey("GroupId")
						.OnDelete(DeleteBehavior.Cascade),
					left => left
						.HasOne<Disciplines>()
						.WithMany()
						.HasForeignKey("DisciplineId")
						.OnDelete(DeleteBehavior.Cascade),
					join =>
					{
						join.HasKey("DisciplineId", "GroupId");
						join.ToTable("DisciplinesGroups");
					});

			// Составной ключ для таблицы LessonMarks
			modelBuilder.Entity<LessonMarks>()
			.HasKey(lessonMark => new
			{
				lessonMark.LessonId,
				lessonMark.StudentId,
				lessonMark.MarkId
			});

			// Составной ключ для таблицы Curators (ProfessorId, GroupId)
			modelBuilder.Entity<Curators>()
				.HasKey(c => new { c.ProfessorId, c.GroupId });

		}

	}
}