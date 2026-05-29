import { LessonTable } from "@/entities/lesson"
import { ComeBackButton } from "@/shared/ui/come-back-button"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import Box from "@mui/material/Box"
import { memo, MouseEventHandler } from "react"
import Typography from "@mui/material/Typography"

interface Props {
	lessons: TLesson[]
	rows: TJournalRow[]
	onBackClick?: MouseEventHandler<HTMLButtonElement>
	title?: string
}

const LessonJournalTable = (props: Props) => {
	const {
		lessons,
		rows,
		onBackClick,
		title,
	} = props

	return (
		<Box className="flex w-fit max-w-full flex-col gap-4">
			<Box className="flex justify-start">
				<ComeBackButton onClick={onBackClick} />
				<Typography variant="h6">{title}</Typography>
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
