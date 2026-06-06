using System.ComponentModel.DataAnnotations;

namespace MainService
{

    public enum UserRole
    {
        Student = 0,
        Professor = 1,
        All = 2
    }

    public class Users : BaseEntity
    {
        [Key]
        public int UserId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string UserUuid { get; set; }

        public StudentPersons? StudentPerson { get; set; }
        public UniversityEmployers? UniversityEmployer { get; set; }

    //  определяет в какой таблице будет храниться пользователь 
    // в студенческой или преподавательской, чтобы не делать два запроса в базу данных
        public UserRole Role { get; set; } = UserRole.Student;

    }
}
