using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class UniversityEmployers
    {
        [Key]
        public int UniversityEmployerId { get; set; }

        public int UserId { get; set; }
        public required Users? User { get; set; }

        public Professors? Professor { get; set; }

    }
}
