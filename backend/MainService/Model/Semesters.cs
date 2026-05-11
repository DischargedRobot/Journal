using System.ComponentModel.DataAnnotations;

namespace MainService
{
	public class Semesters
	{
		[Key]
		public int SemesterId { get; set; }
		public Guid Uuid { get; set; } = Guid.NewGuid();

		[Required]
		public required string SemesterName { get; set; }

		[Required]
		public required int SemesterCode { get; set; }

		public ICollection<Disciplines>? Disciplines { get; set; }
	}
}