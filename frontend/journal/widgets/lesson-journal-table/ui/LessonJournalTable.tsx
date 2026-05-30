import { LessonTable } from "@/entities/lesson"
import { ComeBackButton } from "@/shared/ui/come-back-button"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import Box from "@mui/material/Box"
import { memo, MouseEventHandler } from "react"
import Typography from "@mui/material/Typography"
import { SxProps, Theme } from "@mui/material"
import { TDiscipline } from "@/shared/model/discipline"
import DisciplineTag from "@/entities/discipline/ui/DisciplineTag"

interface Props {
	lessons: TLesson[]
	rows: TJournalRow[]
	onBackClick?: MouseEventHandler<HTMLButtonElement>
	title?: string
	className?: string
	sx?: SxProps<Theme>
	discipline?: TDiscipline
}

const LessonJournalTable = (props: Props) => {
	const {
		lessons,
		rows,
		onBackClick,
		title,
		className,
		sx,
		discipline,
	} = props

	return (
		<Box
			className={`flex w-fit max-w-full flex-col gap-4 ${className}`}
			sx={sx}>
			<Box className="flex justify-start items-center gap-2">
				<ComeBackButton onClick={onBackClick} />
				<Typography className="flex gap-2" variant="h6">
					{discipline?.name ?? title}
					<DisciplineTag disciplineType={discipline?.type ?? "Другое"} />
				</Typography>
			</Box>
			<LessonTable
				lessons={lessons}
				rows={rows}
				showMoreTools={false}
				onHeaderMoreToolsClick={() => { }}
				onRowMoreToolsClick={() => { }}
			/>
		</Box>
	)
}

export default memo(LessonJournalTable)
