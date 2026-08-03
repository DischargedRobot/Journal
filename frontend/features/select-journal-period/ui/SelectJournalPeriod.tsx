import { Combobox, type ComboboxOption } from "@/shared/ui/combobox"
import Box from "@mui/material/Box"

const yearOptions: ComboboxOption<number>[] = [{ value: 2024, label: "2024" }]
const semesterOptions: ComboboxOption<number>[] = [
	{ value: 1, label: "Осенний" },
	{ value: 2, label: "Весений" },
]

interface Props {
	onYearChange?: (value: ComboboxOption<number> | null) => void
	onSemesterChange?: (value: ComboboxOption<number> | null) => void
}

const SelectJournalPeriod = ({ onYearChange, onSemesterChange }: Props) => {
	return (
		<Box className="flex items-start w-full gap-4">
			<Combobox
				options={yearOptions}
				onChange={onYearChange}
				placeholder="Год"
			/>
			<Combobox
				options={semesterOptions}
				onChange={onSemesterChange}
				placeholder="Семестр"
			/>
		</Box>
	)
}

export default SelectJournalPeriod
