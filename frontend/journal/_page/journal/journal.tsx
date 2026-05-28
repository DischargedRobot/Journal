import { TDiscipline } from "@/shared/model/discipline"
import { DisciplineTable } from "@/widgets/discipline-table"
import Autocomplete, { type AutocompleteRenderInputParams } from "@mui/material/Autocomplete"
import Box from "@mui/material/Box"
import TextField from "@mui/material/TextField"
import type { SxProps, Theme } from "@mui/material/styles"

type TOption = { value: number; label: string }

const yearOptions: TOption[] = [{ value: 2024, label: "2024" }]
const semesterOptions: TOption[] = [
	{ value: 1, label: "Осенний" },
	{ value: 2, label: "Весений" },
]

// const open = "&:has([aria-expanded='true'])"

// const autocompleteSx: SxProps<Theme> = {
// 	// текст лейбла
// 	"& .MuiInputLabel-root, & .MuiInputLabel-root.Mui-focused": { color: "text.secondary" },
// 	// текст лейбла при открытом списке
// 	[`${open} .MuiInputLabel-root`]: { color: "primary.main" },
// 	// граница поля
// 	"& .MuiOutlinedInput-notchedOutline": { borderColor: "divider" },
// 	"& .MuiOutlinedInput-root.Mui-focused:hover .MuiOutlinedInput-notchedOutline": { borderColor: "text.primary" },
// 	// граница поля при фокусе
// 	"& .MuiOutlinedInput-root.Mui-focused .MuiOutlinedInput-notchedOutline": {
// 		borderColor: "divider",
// 	},
// 	// граница поля при открытом списке и фокусе
// 	[`${open} .MuiOutlinedInput-root.Mui-focused .MuiOutlinedInput-notchedOutline`]: {
// 		borderColor: "primary.main",
// 	},
// 	// текст поля при открытом списке
// 	[`${open} .MuiInputBase-input`]: { color: "primary.main" },
// }

// const listboxSx: SxProps<Theme> = {
// "& .MuiAutocomplete-option:hover, & .MuiAutocomplete-option[aria-selected='true']": {
// 		color: "primary.main",
// 	},
// }

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
				<Autocomplete
					value={yearOptions[0]}
					onChange={(event, value) => {
						console.log(value)
					}}
					options={yearOptions}
					defaultValue={yearOptions[0]}
					size="medium"
					sx={{
						width: "200px",
					}}
					renderInput={(params) => (
						<TextField
							{...params}
							label="Год"

						/>
					)}
				/>
				<Autocomplete
					value={semesterOptions[0]}
					onChange={(event, value) => {
						console.log(value)
					}}
					options={semesterOptions}
					defaultValue={semesterOptions[0]}
					size="medium"
					sx={{
						width: "200px",
					}}
					renderInput={(params) => (
						<TextField
							{...params}
							label="Семестр"

						/>
					)}
				/>
			</Box>
			<DisciplineTable disciplines={disciplines} />
		</div>
	)
}

export default Journal
