using System.ComponentModel.DataAnnotations;

namespace MainService
{
	public class Semesters
	{
		[Key]
		public int SemesterId { get; set; }

		[Required]
		public required string SemesterName { get; set; }

		[Required]
		public required int SemesterCode { get; set; }
	}
}