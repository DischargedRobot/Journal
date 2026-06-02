import type { ColDef, ColGroupDef, CellClickedEvent } from "ag-grid-community"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import { formatStudentShortName } from "@/shared/model/student"
import { formatLessonDate, getLessonTopic } from "./lessonFormat"
import { HeaderMoreToolsCell, RowMoreToolsCell } from "./LessonGridMenuCells"
export type BuildLessonColumnDefsOptions = {
	showMoreTools?: boolean
}

const PINNED_COL = {
	// запрет закрепления колонок
	lockPinned: true,
	// запрет перемещения колонок
	lockPosition: true,
	suppressMovable: true,
} as const

const PINNED_RIGHT_COL = {
	...PINNED_COL,
	lockPosition: "right",
} as const

const HEADER_CLASS = "lesson-grid-header-cell"
/** Заголовки Б/Н и Оценка — без drag; перестановка блока занятия — по групповым заголовкам */
const LESSON_LEAF_HEADER_CLASS = `${HEADER_CLASS} lesson-grid-lesson-leaf-header lesson-grid-no-resize`

// Группа колонок занятия
const buildLessonGroup = (
	lesson: TLesson,
	onPresenceCellClick?: (
		params: CellClickedEvent<TJournalRow>,
		lesson: TLesson,
	) => void,
): ColGroupDef<TJournalRow> => ({
	groupId: `lesson-${lesson.uuid}`,
	headerName: `№${lesson.code} ${formatLessonDate(lesson.startDate)}`,
	marryChildren: true,
	headerClass: HEADER_CLASS,
	children: [
		{
			headerName: getLessonTopic(lesson),
			marryChildren: true,
			lockPinned: true,
			headerClass: HEADER_CLASS,
			children: [
				{
					colId: `${lesson.uuid}_presence`,
					headerName: "Б/Н",
					minWidth: 64,
					lockPinned: true,
					width: 64,
					headerClass: LESSON_LEAF_HEADER_CLASS,
					cellClass: "lesson-grid-cell-center",
					valueGetter: ({ data }) =>
						data?.lessons.get(lesson.uuid)?.presenceStatus ?? "",
					onCellClicked: onPresenceCellClick
						? (params) => onPresenceCellClick(params, lesson)
						: undefined,
				},
				{
					colId: `${lesson.uuid}_grade`,
					headerName: "Оценка",
					lockPinned: true,
					minWidth: 90,
					width: 90,
					headerClass: LESSON_LEAF_HEADER_CLASS,
					cellClass: "lesson-grid-cell-center",
					valueGetter: ({ data }) =>
						data?.lessons.get(lesson.uuid)?.mark ?? "",
				},
			],
		},
	],
})

const buildMoreToolsColumn = (): ColDef<TJournalRow> => ({
	colId: "rowMenu",
	headerName: "",
	pinned: "right",
	width: 48,
	...PINNED_RIGHT_COL,
	sortable: false,
	filter: false,
	resizable: false,
	suppressNavigable: true,
	cellClass: "lesson-grid-more-tools-cell",
	headerClass: `${HEADER_CLASS} lesson-grid-more-tools-cell`,
	headerComponent: HeaderMoreToolsCell,
	cellRenderer: RowMoreToolsCell,
})

// Все колонки таблицы
export const buildLessonColumnDefs = (
	orderedLessons: TLesson[],
	onPresenceCellClick?: (
		params: CellClickedEvent<TJournalRow>,
		lesson: TLesson,
	) => void,
	{ showMoreTools = false }: BuildLessonColumnDefsOptions = {},
): (ColDef<TJournalRow> | ColGroupDef<TJournalRow>)[] => [
	{
		field: "order",
		colId: "order",
		headerName: "№",
		width: 56,
		minWidth: 56,
		...PINNED_COL,
		resizable: false,
		headerClass: HEADER_CLASS,
	},
	{
		colId: "fullName",
		headerName: "Фамилия И.О.",
		width: 160,
		minWidth: 140,
		...PINNED_COL,
		headerClass: HEADER_CLASS,
		valueGetter: ({ data }) =>
			data ? formatStudentShortName(data.student) : "",
	},
	...orderedLessons.map((lesson) =>
		buildLessonGroup(lesson, onPresenceCellClick),
	),
	{
		field: "percent",
		colId: "percent",
		headerName: "%",
		resizable: false,
		width: 72,
		minWidth: 72,
		...PINNED_RIGHT_COL,
		headerClass: HEADER_CLASS,
		cellClass: "lesson-grid-cell-center",
	},
	{
		field: "attendedTotal",
		colId: "attendedTotal",
		headerName: "Был/все",
		width: 88,
		minWidth: 88,
		...PINNED_RIGHT_COL,
		headerClass: HEADER_CLASS,
		cellClass: "lesson-grid-cell-center",
	},
	{
		field: "attestation",
		colId: "attestation",
		headerName: "Аттестация",
		width: 100,
		minWidth: 100,
		...PINNED_RIGHT_COL,
		headerClass: HEADER_CLASS,
		cellClass: "lesson-grid-cell-center",
	},
	...(showMoreTools ? [buildMoreToolsColumn()] : []),
]
