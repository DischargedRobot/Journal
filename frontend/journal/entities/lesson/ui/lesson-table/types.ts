import type { ComponentType } from "react"
import type { TJournalRow } from "@/shared/model/lesson"

// Пропсы кнопки ⋮, которую рендерит таблица в шапке и в строках.
export type LessonMoreToolsButtonProps = {
	onClick: () => void
	// Нет `row` — кнопка в шапке; есть — в строке студента.
	row?: TJournalRow
	className?: string
}

export type LessonGridContext = {
	onHeaderMoreToolsClick?: () => void
	onRowMoreToolsClick?: (row: TJournalRow) => void
	moreToolsButton: ComponentType<LessonMoreToolsButtonProps>
}
