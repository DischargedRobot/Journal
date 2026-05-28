import { TDiscipline } from "@/shared/model/discipline"
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
			<DisciplineTable disciplines={disciplines} />
		</div>
	)
}

export default Journal
