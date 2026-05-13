using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace MainService
{
    [PrimaryKey(nameof(LessonTypeId), nameof(MarkTypeId), nameof(DisciplineId))]
    public class SelectedMarkTypes : BaseEntity
    {

        public required int LessonTypeId { get; set; }
        [ForeignKey("LessonTypeId")]
        public LessonTypes? LessonType { get; set; }

        public required int MarkTypeId { get; set; }
        [ForeignKey("MarkTypeId")]
        public MarkTypes? MarkType { get; set; }

        public required int DisciplineId { get; set; }
        [ForeignKey("DisciplineId")]
        public Disciplines? Discipline { get; set; }

    }
}
