using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace MainService
{
    [PrimaryKey(nameof(LessonTypeId), nameof(MarkTypeId), nameof(DisciplineId))]
    public class SelectedMarkTypes : BaseEntity
    {

        public int LessonTypeId { get; set; } = 0;
        [ForeignKey("LessonTypeId")]
        public LessonTypes? LessonType { get; set; }

        public int MarkTypeId { get; set; } = 0;
        [ForeignKey("MarkTypeId")]
        public MarkTypes? MarkType { get; set; }

        public required int DisciplineId { get; set; }
        [ForeignKey("DisciplineId")]
        public Disciplines? Disciplines { get; set; }

    }
}
