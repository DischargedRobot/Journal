using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class StudentPersons
    {
        [Key]
        public int StudentPersonId { get; set; }

        public int UserId { get; set; }
        public required Users User { get; set; }
    }
}