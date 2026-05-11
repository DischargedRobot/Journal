using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace MainService
{
    [PrimaryKey(nameof(ProfessorId), nameof(GroupId))]
    public class Curators
    {
        [Required] //  каскадное удаление
        public required int ProfessorId { get; set; }
        [ForeignKey("ProfessorId")]
        public Professors? Professor { get; set; }

        [Required]
        public required int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Groups? Group { get; set; }
    }
}