using MainService;
using Microsoft.EntityFrameworkCore;

namespace MainService.Tests.Helpers;

public static class TestDataMock
{
    public static async Task<Faculties> MockFacultyAsync(MainServiceContext context)
    {
        Faculties faculty = new()
        {
            Name = "Факультет информатики",
            ShortName = "ФИ"
        };
        context.Faculties.Add(faculty);
        await context.SaveChangesAsync();
        return faculty;
    }

    public static async Task<TrainingDirections> MockTrainingDirectionAsync(MainServiceContext context)
    {
        TrainingDirections trainingDirection = new()
        {
            Name = "Программная инженерия",
            Code = "09.03.04"
        };
        context.TrainingDirections.Add(trainingDirection);
        await context.SaveChangesAsync();
        return trainingDirection;
    }

    public static async Task<Groups> MockGroupAsync(MainServiceContext context)
    {
        Faculties faculty = await MockFacultyAsync(context);
        TrainingDirections trainingDirection = await MockTrainingDirectionAsync(context);

        Groups group = new()
        {
            Code = "ИВТ-101",
            AdmissionDate = new DateOnly(2024, 9, 1),
            TrainingDirectionId = trainingDirection.TrainingDirectionId,
            TrainingDirection = trainingDirection,
            FacultyId = faculty.FacultyId,
            Faculty = faculty,
            Curators = []
        };
        context.Groups.Add(group);
        await context.SaveChangesAsync();
        return group;
    }

    public static async Task<Students> MockStudentAsync(MainServiceContext context)
    {
        Groups group = await MockGroupAsync(context);

        Users user = new()
        {
            UserUuid = Guid.NewGuid().ToString()
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        StudentPersons studentPerson = new()
        {
            FirstName = "Иван",
            LastName = "Иванов",
            Patronymic = "Иванович",
            UserId = user.UserId,
            User = user
        };
        context.StudentPersons.Add(studentPerson);
        await context.SaveChangesAsync();

        Students student = new()
        {
            StudentCode = 12345,
            StudentPersonId = studentPerson.StudentPersonId,
            StudentPerson = studentPerson,
            GroupId = group.GroupId,
            Group = group
        };
        context.Students.Add(student);
        await context.SaveChangesAsync();
        return student;
    }

    public static async Task<Disciplines> MockDisciplineAsync(MainServiceContext context)
    {
        Groups group = await MockGroupAsync(context);

        Semesters semester = new()
        {
            SemesterName = "Осенний",
            SemesterCode = 1
        };
        context.Semesters.Add(semester);

        AcademicYears academicYear = new()
        {
            Date = new DateOnly(2024, 9, 1)
        };
        context.AcademicYears.Add(academicYear);

        DisciplinesTypes disciplineType = new()
        {
            Name = "Лекция",
            ShortName = "Лек"
        };
        context.DisciplinesTypes.Add(disciplineType);

        LessonTypes lessonType = new()
        {
            Name = "Лекция",
            ShortName = "Лек"
        };
        context.LessonTypes.Add(lessonType);

        MarkTypes markType = new()
        {
            Name = "Оценка"
        };
        context.MarkTypes.Add(markType);
        await context.SaveChangesAsync();

        Disciplines discipline = new()
        {
            Name = "Математический анализ",
            ShortName = "Мат",
            IsArchived = false,
            SemesterId = semester.SemesterId,
            Semester = semester,
            AcademicYearId = academicYear.AcademicYearId,
            AcademicYear = academicYear,
            DisciplineTypeId = disciplineType.DisciplineTypeId,
            DisciplinesTypes = disciplineType,
            Groups = [group],
            Professors = [],
            SelectedMarkTypes = []
        };
        context.Disciplines.Add(discipline);
        await context.SaveChangesAsync();

        discipline.SelectedMarkTypes.Add(new SelectedMarkTypes
        {
            DisciplineId = discipline.DisciplineId,
            Disciplines = discipline,
            LessonTypeId = lessonType.LessonTypeId,
            LessonType = lessonType,
            MarkTypeId = markType.MarkTypeId,
            MarkType = markType
        });
        await context.SaveChangesAsync();

        return discipline;
    }
}
