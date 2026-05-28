import type { ColDef, ColGroupDef } from "ag-grid-community"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import { formatLessonDate, getLessonTopic } from "./lessonFormat"
import { HeaderMoreToolsCell, RowMoreToolsCell } from "./LessonGridMenuCells"

export type BuildLessonColumnDefsOptions = {
	showMoreTools?: boolean
}

const PINNED_COL = {
	lockPinned: true,
	suppressMovable: true,
} as const

const HEADER_CLASS = "lesson-grid-header-cell"

// Группа колонок занятия
const buildLessonGroup = (lesson: TLesson): ColGroupDef<TJournalRow> => ({
	groupId: `lesson-${lesson.uuid}`,
	headerName: `№${lesson.code} ${formatLessonDate(lesson.startDate)}`,
	marryChildren: true,
	headerClass: HEADER_CLASS,
	children: [
		{
			headerName: getLessonTopic(lesson),
			headerClass: HEADER_CLASS,
			children: [
				{
					colId: `${lesson.uuid}_presence`,
					headerName: "Б/Н",
					width: 72,
					minWidth: 64,
					headerClass: HEADER_CLASS,
					cellClass: "lesson-grid-cell-center",
					valueGetter: ({ data }) =>
						data?.cells[lesson.uuid]?.presence ?? "",
				},
				{
					colId: `${lesson.uuid}_grade`,
					headerName: "Оценка",
					width: 72,
					minWidth: 64,
					headerClass: HEADER_CLASS,
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
	maxWidth: 48,
	...PINNED_COL,
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
		headerClass: HEADER_CLASS,
		cellClass: "lesson-grid-cell-left",
	},
	...orderedLessons.map(buildLessonGroup),
	{
		field: "percent",
		colId: "percent",
		headerName: "%",
		pinned: "right",
		width: 72,
		...PINNED_COL,
		headerClass: HEADER_CLASS,
		cellClass: "lesson-grid-cell-center",
	},
	{
		field: "attendedTotal",
		colId: "attendedTotal",
		headerName: "Был/все",
		pinned: "right",
		width: 88,
		...PINNED_COL,
		headerClass: HEADER_CLASS,
		cellClass: "lesson-grid-cell-center",
	},
	{
		field: "attestation",
		colId: "attestation",
		headerName: "Аттестация",
		pinned: "right",
		width: 100,
		...PINNED_COL,
		headerClass: HEADER_CLASS,
		cellClass: "lesson-grid-cell-center",
	},
	...(showMoreTools ? [buildMoreToolsColumn()] : []),
]
