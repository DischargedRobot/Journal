using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class StudentPersons : BaseEntity
    {
        [Key]
        public int StudentPersonId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        //  ФИО нужно чтобы не джоинить ещё и юзеров
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Patronymic { get; set; }

        public required int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? User { get; set; }

        public ICollection<Students>? Students { get; set; } = [];
    }
}