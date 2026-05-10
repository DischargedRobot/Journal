using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class Curators
    {
        public required int ProfessorId { get; set; }
        [ForeignKey("ProfessorId")]
        public Professors? Professor { get; set; }
        public ICollection<Groups>? Groups { get; set; }
    }
}