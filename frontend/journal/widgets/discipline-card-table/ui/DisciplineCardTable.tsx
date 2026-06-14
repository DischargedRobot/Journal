import { DisciplineCard } from "@/entities/discipline"
import { TDiscipline } from "@/shared/model/discipline"
import Box from "@mui/material/Box"
import List from "@mui/material/List"
import ListItem from "@mui/material/ListItem"
import Typography from "@mui/material/Typography"
import { SelectJournalPeriod } from "@/features/select-journal-period"
import { MouseEvent } from "react"
import { SxProps, Theme } from "@mui/material"
import { ComboboxOption } from "@/shared/ui/combobox/Combobox"

interface Props {
	disciplines: TDiscipline[]
	onDisciplineClick?: (
		discipline: TDiscipline,
		e: MouseEvent<HTMLDivElement>,
	) => void
	sx?: SxProps<Theme>
	className?: string
}

const DisciplineCardTable = ({
	disciplines,
	onDisciplineClick,
	className,
	sx,
}: Props) => {
	// группировка дисциплин по названию
	const grouped = disciplines.reduce<Record<string, TDiscipline[]>>(
		(acc, discipline) => {
			const displayName =
				discipline.name.length > 12
					? discipline.shortName
					: discipline.name

			if (!acc[displayName]) {
				acc[displayName] = []
			}

			acc[displayName].push(discipline)

			return acc
		},
		{},
	)

	const handleYearChange = (year: ComboboxOption<number> | null) => {
		console.log(year)
	}

	const handleSemesterChange = (semester: ComboboxOption<number> | null) => {
		console.log(semester)
	}

	return (
		<Box className={`flex flex-col w-fit gap-4 ${className}`} sx={sx}>
			<SelectJournalPeriod
				onYearChange={handleYearChange}
				onSemesterChange={handleSemesterChange}
			/>
			<Box className="flex gap-4">
				{Object.entries(grouped).map(([name, disciplines]) => (
					<div
						key={name}
						className="flex flex-col items-center gap-1 w-[150px]"
					>
						<Typography
							className="font-bold w-full"
							sx={{ color: "primary.dark" }}
							noWrap
							align="center"
							variant="h6"
						>
							{name}
						</Typography>
						<List className="flex flex-col gap-5 w-full">
							{disciplines.map((discipline) => (
								<ListItem key={discipline.uuid} disablePadding>
									<DisciplineCard
										discipline={discipline}
										onClick={(discipline, e) =>
											onDisciplineClick?.(discipline, e)
										}
									/>
								</ListItem>
							))}
						</List>
					</div>
				))}
			</Box>
		</Box>
	)
}

export default DisciplineCardTable
