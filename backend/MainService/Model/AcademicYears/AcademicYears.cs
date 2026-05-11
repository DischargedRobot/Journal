using System.ComponentModel.DataAnnotations;

namespace MainService
{
	public class AcademicYears
	{
		[Key]
		public required int AcademicYearId { get; set; }

		[Required]
		public Guid Uuid { get; set; } = Guid.NewGuid();

		[Required]
		public required DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

		public ICollection<Disciplines>? Disciplines { get; set; } = [];
	}
}