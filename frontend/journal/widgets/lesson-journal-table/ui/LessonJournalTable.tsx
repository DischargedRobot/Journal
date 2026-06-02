import { ComeBackButton } from "@/shared/ui/come-back-button"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import type { TStudent } from "@/shared/model/student"
import Box from "@mui/material/Box"
import { memo, MouseEventHandler, SyntheticEvent, useCallback, useRef, useState } from "react"
import Typography from "@mui/material/Typography"
import { SxProps, Theme } from "@mui/material"
import { TDiscipline } from "@/shared/model/discipline"
import DisciplineTag from "@/entities/discipline/ui/DisciplineTag"
import { SelectPresencesStatus } from "@/features/select-presences-status"
import { TPresencesStatus } from "@/shared/model/presences-status"
import { LessonTable } from "@/entities/lesson"
import type { CellClickedEvent } from "ag-grid-community"

interface Props {
	lessons: TLesson[]
	rows: TJournalRow[]
	onBackClick?: MouseEventHandler<HTMLButtonElement>
	title?: string
	className?: string
	sx?: SxProps<Theme>
	discipline?: TDiscipline
}

const NOOP = () => { }

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

	const [selectPresencesStatusAnchorEl, setSelectPresencesStatusAnchorEl] = useState<HTMLElement | null>(null)

	const [selectedPresencesStatus, setSelectedPresencesStatus] = useState<TPresencesStatus>("О")
	const [journalRows, setJournalRows] = useState<TJournalRow[]>(rows)
	const [selectedLesson, setSelectedLesson] = useState<{
		lesson: TLesson
		student: TStudent
	} | null>(null)
	const lockedRowRef = useRef<HTMLElement | null>(null)

	const unlockLockedRow = useCallback(() => {
		lockedRowRef.current?.classList.remove("locked")
		lockedRowRef.current = null
	}, [])

	const handlePresenceCellClick = useCallback(
		(params: CellClickedEvent<TJournalRow>, lesson: TLesson) => {
			params.event?.stopPropagation()
			const target = params.event?.target
			const cellEl =
				target instanceof Element
					? target.closest<HTMLElement>(".ag-cell")
					: null

			if (!cellEl || !cellEl.isConnected || cellEl.getClientRects().length === 0) {
				unlockLockedRow()
				setSelectPresencesStatusAnchorEl(null)
				return
			}
			if (!params.data) {
				return
			}
			const rowEl = cellEl.closest<HTMLElement>(".ag-row")
			if (rowEl && rowEl !== lockedRowRef.current) {
				unlockLockedRow()
				rowEl.classList.add("locked")
				lockedRowRef.current = rowEl
			}

			setSelectedPresencesStatus(params.data.lessons.get(lesson.uuid)?.presenceStatus ?? "О")
			setSelectedLesson({
				lesson,
				student: params.data.student,
			})
			setSelectPresencesStatusAnchorEl(cellEl)
		},
		[unlockLockedRow],
	)

	const handleStatusChange = useCallback(
		(event: SyntheticEvent, status: TPresencesStatus) => {
			event.stopPropagation()
			if (!selectedLesson) {
				return
			}
			setJournalRows((prevRows) =>
				prevRows.map((row) => {
					if (row.student.uuid !== selectedLesson.student.uuid) {
						return row
					}

					const lessonEntry = row.lessons.get(selectedLesson.lesson.uuid)
					// if (!lessonEntry) {
					// 	return row
					// }

					const nextLessons = new Map(row.lessons)
					nextLessons.set(selectedLesson.lesson.uuid, {
						mark: lessonEntry?.mark ?? "",
						presenceStatus: status,
					})

					return {
						...row,
						lessons: nextLessons,
						student: {
							...row.student,
							lessons: nextLessons,
						},
					}
				}),
			)
			setSelectedPresencesStatus(status)
			setSelectedLesson(null)
			unlockLockedRow()
			setSelectPresencesStatusAnchorEl(null)
		},
		[selectedLesson, unlockLockedRow],
	)

	const handleSelectPresencesStatusClose = useCallback(() => {
		setSelectedLesson(null)
		unlockLockedRow()
		setSelectPresencesStatusAnchorEl(null)
	}, [unlockLockedRow])

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
				rows={journalRows}
				showMoreTools={false}
				onHeaderMoreToolsClick={NOOP}
				onRowMoreToolsClick={NOOP}
				onPresenceCellClick={handlePresenceCellClick}
			/>
			<SelectPresencesStatus
				isOpen={Boolean(selectPresencesStatusAnchorEl)}
				anchorEl={selectPresencesStatusAnchorEl}
				onClose={handleSelectPresencesStatusClose}
				onChange={handleStatusChange}
				selectedStatus={selectedPresencesStatus}
				absenceStatusDenominator={2}
			/>
		</Box>
	)
}

export default memo(LessonJournalTable)
