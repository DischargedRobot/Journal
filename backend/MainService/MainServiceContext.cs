using Microsoft.EntityFrameworkCore;

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
		public DbSet<LessonTypes> LessonTypes => Set<LessonTypes>();
		public DbSet<Marks> Marks => Set<Marks>();
		public DbSet<EmployeePosts> EmployeePosts => Set<EmployeePosts>();
		public DbSet<PresenceStatuses> PresenceStatuses => Set<PresenceStatuses>();
		public DbSet<Professors> Professors => Set<Professors>();
		public DbSet<Semesters> Semesters => Set<Semesters>();
		public DbSet<Students> Students => Set<Students>();
		public DbSet<StudentPersons> StudentPersons => Set<StudentPersons>();
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

			// Таблица кураторов
			modelBuilder.Entity<Groups>()
				.HasMany(group => group.Curators)
				.WithMany(professor => professor.GroupCurator)
				.UsingEntity<Curators>(
					right => right
						.HasOne(curator => curator.Professor)
						.WithMany()
						.HasForeignKey(curator => curator.ProfessorId)
						.OnDelete(DeleteBehavior.Cascade),
					left => left
						.HasOne(curator => curator.Group)
						.WithMany()
						.HasForeignKey(curator => curator.GroupId)
						.OnDelete(DeleteBehavior.Cascade),
					join =>
					{
						join.HasKey(curator => new { curator.ProfessorId, curator.GroupId });
						join.ToTable("Curators");
					});

			// Альтернативные ключи (UUID) для всех сущностей
			modelBuilder.Entity<AcademicYears>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Attestations>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<AttestationMarks>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<AttestationTypes>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Brigades>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Departments>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Disciplines>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<DisciplinesRegisters>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<EmployeePosts>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Faculties>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Groups>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Lessons>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<LessonTypes>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<LessonPresences>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Marks>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<MarkTypes>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<NotesAboutStudent>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<PresenceStatuses>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Professors>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Semesters>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<StudentPersons>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Students>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<TrainingDirections>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<UniversityEmployers>().HasAlternateKey(e => e.Uuid);
			modelBuilder.Entity<Users>().HasAlternateKey(e => e.Uuid);
		}

		private void IncrementVersions()
		{
			foreach (var entry in ChangeTracker.Entries<BaseEntity>()
				.Where(e => e.State == EntityState.Added ||
					(e.State == EntityState.Modified &&
					 e.Properties.Any(p => p.IsModified && p.Metadata.Name != nameof(BaseEntity.Version)))))
			{
				entry.Entity.Version++;
			}
		}

		public override int SaveChanges()
		{
			IncrementVersions();
			return base.SaveChanges();
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			IncrementVersions();
			return base.SaveChangesAsync(cancellationToken);
		}
	}
}
