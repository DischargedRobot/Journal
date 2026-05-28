import { LessonTable } from "@/entities/lesson"
import { TDiscipline } from "@/shared/model/discipline"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import { Combobox, type ComboboxOption } from "@/shared/ui/combobox"
import { DisciplineTable } from "@/widgets/discipline-table"
import Box from "@mui/material/Box"

const yearOptions: ComboboxOption<number>[] = [{ value: 2024, label: "2024" }]
const semesterOptions: ComboboxOption<number>[] = [
	{ value: 1, label: "Осенний" },
	{ value: 2, label: "Весений" },
]

const disciplines: TDiscipline[] = [
	{
		uuid: "1",
		name: "Математика",
		shortName: "Мат.",
		type: "Лекция",
		isArchived: false,
		professorUuid: "1",
		groupUuid: "1",
		DisciplinesSet: "1",
	},
	{
		uuid: "2",
		name: "Физика",
		shortName: "Физ.",
		type: "Упражнение",
		isArchived: false,
		professorUuid: "2",
		groupUuid: "1",
		DisciplinesSet: "1",
	},
	{
		uuid: "3",
		name: "История",
		shortName: "Ист.",
		type: "Семинар",
		isArchived: false,
		professorUuid: "3",
		groupUuid: "2",
		DisciplinesSet: "1",
	},
	{
		uuid: "4",
		name: "Программирование",
		shortName: "Прог.",
		type: "Лабораторная",
		isArchived: false,
		professorUuid: "4",
		groupUuid: "2",
		DisciplinesSet: "2",
	},
	{
		uuid: "7",
		name: "Программирование",
		shortName: "Прог.",
		type: "Упражнение",
		isArchived: false,
		professorUuid: "4",
		groupUuid: "2",
		DisciplinesSet: "2",
	},
	{
		uuid: "5",
		name: "Английский язык",
		shortName: "АЯ",
		type: "Лекция",
		isArchived: true,
		professorUuid: "5",
		groupUuid: "3",
		DisciplinesSet: "2",
	},

]

const lessonTopic = "Преобразование Фурье"

const mockLessons: TLesson[] = Array.from({ length: 4 }, (_, index) => ({
	uuid: `lesson-${index + 1}`,
	code: 13,
	startDate: "2024-12-25T10:00:00Z",
	name: lessonTopic,
	shortName: "Преобр. Фурье",
	lessonTypeUuid: "lesson-type-1",
	disciplineUuid: "1",
	version: 0,
}))

const emptyCells = Object.fromEntries(
	mockLessons.map((lesson) => [
		lesson.uuid,
		{ presence: "", grade: "" },
	]),
)

const mockJournalRows: TJournalRow[] = [
	{
		uuid: "student-1",
		order: 1,
		fullName: "Фамилия И. О.",
		cells: {
			...emptyCells,
			"lesson-1": { presence: "Н", grade: "зачтено" },
			"lesson-2": { presence: "Н", grade: "зачтено" },
			"lesson-3": { presence: "Н", grade: "зачтено" },
			"lesson-4": { presence: "Н", grade: "зачтено" },
		},
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		uuid: "student-2",
		order: 5,
		fullName: "Фамилия И. О.",
		cells: {
			...emptyCells,
			"lesson-1": { presence: "1/2", grade: "зачтено" },
			"lesson-2": { presence: "1/2", grade: "зачтено" },
			"lesson-3": { presence: "1/2", grade: "зачтено" },
			"lesson-4": { presence: "1/2", grade: "зачтено" },
		},
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		uuid: "student-3",
		order: 3,
		fullName: "Фамилия И. О.",
		cells: {
			...emptyCells,
			"lesson-1": { presence: "Б", grade: "5" },
			"lesson-2": { presence: "Б", grade: "5" },
			"lesson-3": { presence: "Б", grade: "5" },
			"lesson-4": { presence: "Б", grade: "5" },
		},
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		uuid: "student-4",
		order: 2,
		fullName: "Фамилия И. О.",
		cells: emptyCells,
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		uuid: "student-5",
		order: 4,
		fullName: "Фамилия И. О.",
		cells: emptyCells,
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
]

const Journal = () => {
	return (
		<div className="flex flex-col  items-center justify-center gap-4  p-4 w-fit overflow-auto ">
			<Box className="flex items-star  w-full gap-4">
				<Combobox
					label="Год"
					options={yearOptions}
					defaultValue={yearOptions[0]}
					onChange={(value) => {
						console.log(value)
					}}
				/>
				<Combobox
					label="Семестр"
					options={semesterOptions}
					defaultValue={semesterOptions[0]}
					onChange={(value) => {
						console.log(value)
					}}
				/>
			</Box>
			<LessonTable lessons={mockLessons} rows={mockJournalRows} />
			<DisciplineTable disciplines={disciplines} />
		</div>
	)
}

export default Journal
