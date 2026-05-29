"use client"

import { DisciplineCard } from "@/entities/discipline"
import { LessonTable } from "@/entities/lesson"
import { SelectJournalPeriod } from "@/features/select-journal-period"
import { TDiscipline } from "@/shared/model/discipline"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import Box from "@mui/material/Box"
import List from "@mui/material/List"
import ListItem from "@mui/material/ListItem"
import Typography from "@mui/material/Typography"
import { useMemo, useState } from "react"

interface Props {
	disciplines: TDiscipline[]
	lessons: TLesson[]
	rows: TJournalRow[]
}

const DisciplineTable = ({ disciplines, lessons, rows }: Props) => {
	const [selectedDiscipline, setSelectedDiscipline] = useState<TDiscipline | null>(null)

	const selectedLessons = useMemo(
		() =>
			selectedDiscipline
				? lessons.filter(
					(lesson) => lesson.disciplineUuid === selectedDiscipline.uuid,
				)
				: [],
		[lessons, selectedDiscipline],
	)

	// группировка дисциплин по названию
	const grouped = disciplines.reduce<Record<string, TDiscipline[]>>(
		(acc, discipline) => {
			// чтобы всё нормально умещалось в пределах столбца
			const displayName =
				discipline.name.length > 12 ? discipline.shortName : discipline.name

			if (!acc[displayName]) {
				acc[displayName] = []
			}

			acc[displayName].push(discipline)

			return acc
		},
		{},
	)

	// выбор дисциплины
	const onClick = (discipline: TDiscipline) => {
		setSelectedDiscipline((current) =>
			current?.uuid === discipline.uuid ? null : discipline,
		)
	}

	return (
		<Box className="flex flex-col gap-4 w-full">
			{selectedDiscipline
				? (
					<LessonTable
						lessons={selectedLessons}
						rows={rows}
						showMoreTools={false}
						onHeaderMoreToolsClick={() => { }}
						onRowMoreToolsClick={() => { }}
					/>
				) : (
					<Box className="flex flex-col gap-4">
						<SelectJournalPeriod />
						<Box className="flex gap-4">
							{Object.entries(grouped).map(([name, disciplines]) => (
								<div key={name} className="flex flex-col items-center gap-1 w-[150px]">
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
										{disciplines.map((discipline) => {

											return (
												<ListItem key={discipline.uuid} disablePadding>

													<DisciplineCard
														discipline={discipline}
														onClick={() => onClick(discipline)}
													/>
												</ListItem>
											)
										})}
									</List>
								</div>
							))}
						</Box>
					</Box>
				)}
		</Box>
	)
}

export default DisciplineTable
