import type { ColDef, ColGroupDef } from "ag-grid-community"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
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
const LESSON_LEAF_HEADER_CLASS = `${HEADER_CLASS} lesson-grid-lesson-leaf-header`

// Группа колонок занятия
const buildLessonGroup = (lesson: TLesson): ColGroupDef<TJournalRow> => ({
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
						data?.cells[lesson.uuid]?.presence ?? "",
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
						data?.cells[lesson.uuid]?.grade ?? "",
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
	{ showMoreTools = false }: BuildLessonColumnDefsOptions = {},
): (ColDef<TJournalRow> | ColGroupDef<TJournalRow>)[] => [
	{
		field: "order",
		colId: "order",
		headerName: "№",
		pinned: "left",
		width: 56,
		minWidth: 56,
		resizable: false,
		...PINNED_COL,
		headerClass: HEADER_CLASS,
		cellClass: "lesson-grid-cell-left lesson-grid-order-cell",
	},
	{
		field: "fullName",
		colId: "fullName",
		headerName: "Фамилия И.О.",
		pinned: "left",
		width: 160,
		minWidth: 140,
		...PINNED_COL,
		lockPosition: "left",
		headerClass: HEADER_CLASS,
		cellClass: "lesson-grid-cell-left",
	},
	...orderedLessons.map(buildLessonGroup),
	{
		field: "percent",
		colId: "percent",
		headerName: "%",
		pinned: "right",
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
		pinned: "right",
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
		pinned: "right",
		width: 100,
		minWidth: 100,
		...PINNED_RIGHT_COL,
		headerClass: HEADER_CLASS,
		cellClass: "lesson-grid-cell-center",
	},
	...(showMoreTools ? [buildMoreToolsColumn()] : []),
]
