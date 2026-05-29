import type { Column, GridApi } from "ag-grid-community"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"

const LESSON_LEAF_COL_ID = /^(.+)_(presence|grade)$/

/** uuid занятия из colId листа (`*_presence` / `*_grade`) или группы (`lesson-*`). */
export const resolveLessonUuidFromColumnId = (colId: string): string | null => {
	const leafMatch = colId.match(LESSON_LEAF_COL_ID)
	if (leafMatch) {
		return leafMatch[1]
	}
	if (colId.startsWith("lesson-")) {
		return colId.slice("lesson-".length)
	}
	return null
}

export const resolveLessonUuidFromColumn = (
	column: Column<TJournalRow> | null | undefined,
): string | null => {
	if (!column) {
		return null
	}
	return resolveLessonUuidFromColumnId(column.getColId())
}

/** Порядок занятий по позиции блоков (Б/Н + Оценка) в сетке. */
export const extractLessonOrder = (api: GridApi<TJournalRow>): string[] => {
	const order: string[] = []
	const seen = new Set<string>()

	for (const col of api.getAllDisplayedColumns() ?? []) {
		const lessonUuid = resolveLessonUuidFromColumnId(col.getColId())
		if (!lessonUuid || seen.has(lessonUuid)) {
			continue
		}
		seen.add(lessonUuid)
		order.push(lessonUuid)
	}

	return order
}

/** У каждого занятия подряд: Б/Н, затем Оценка. */
export const isLessonPairLayoutValid = (api: GridApi<TJournalRow>): boolean => {
	const displayedColumns = api.getAllDisplayedColumns() ?? []

	for (let i = 0; i < displayedColumns.length; i++) {
		const colId = displayedColumns[i].getColId()
		if (!colId.endsWith("_presence")) {
			continue
		}
		const lessonUuid = colId.replace(/_presence$/, "")
		const nextColId = displayedColumns[i + 1]?.getColId()
		if (nextColId !== `${lessonUuid}_grade`) {
			return false
		}
	}

	return true
}

export const restoreLessonColumnOrder = (
	api: GridApi<TJournalRow>,
	lessons: TLesson[],
	showMoreTools: boolean,
): void => {
	const colIds = [
		"order",
		"fullName",
		...lessons.flatMap((lesson) => [
			`${lesson.uuid}_presence`,
			`${lesson.uuid}_grade`,
		]),
		"percent",
		"attendedTotal",
		"attestation",
		...(showMoreTools ? ["rowMenu"] : []),
	]

	api.applyColumnState({
		state: colIds
			.filter((colId) => api.getColumn(colId) != null)
			.map((colId, order) => ({ colId, order })),
		applyOrder: true,
	})
}
