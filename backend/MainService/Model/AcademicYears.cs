using System.ComponentModel.DataAnnotations;

namespace MainService
{
	public class AcademicYears
	{
		[Key]
		public int AcademicYearId { get; set; }

		[Required]
		public required DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

		public ICollection<Disciplines>? Disciplines { get; set; } = [];
	}
}