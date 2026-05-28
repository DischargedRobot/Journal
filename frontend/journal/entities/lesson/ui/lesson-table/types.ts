import type { ComponentType } from "react"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"

/** Пропсы кнопки ⋮, которую рендерит таблица в шапке и в строках. */
export type LessonMoreToolsButtonProps = {
	onClick: () => void
	/** Нет `row` — кнопка в шапке; есть — в строке студента. */
	row?: TJournalRow
}

export type LessonTableProps = {
	lessons: TLesson[]
	rows: TJournalRow[]
	/** Контролируемый набор выбранных строк (uuid студентов). */
	selectedRowUuids?: string[]
	onRowSelect?: (uuids: string[]) => void
	/** Клик по ⋮ в шапке (если не обрабатывается внутри `moreToolsButton`). */
	onHeaderMoreToolsClick?: () => void
	/** Клик по ⋮ в строке (если не обрабатывается внутри `moreToolsButton`). */
	onRowMoreToolsClick?: (row: TJournalRow) => void
	/** Показать колонку ⋮ (по умолчанию `true`). */
	moreTools?: boolean
	/** Кастомная кнопка ⋮; по умолчанию — встроенная иконка. */
	moreToolsButton?: ComponentType<LessonMoreToolsButtonProps>
}

export type LessonGridContext = {
	onHeaderMoreToolsClick?: () => void
	onRowMoreToolsClick?: (row: TJournalRow) => void
	moreToolsButton: ComponentType<LessonMoreToolsButtonProps>
}
