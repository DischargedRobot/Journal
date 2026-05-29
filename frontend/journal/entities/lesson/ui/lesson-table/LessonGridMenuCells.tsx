"use client"

import type { ICellRendererParams, IHeaderParams } from "ag-grid-community"
import type { TJournalRow } from "@/shared/model/lesson"
import type { LessonGridContext } from "./types"

const MORE_TOOLS_CLASS = "lesson-grid-more-tools"

// используется для рендера кнопки ⋮ в шапке
export const HeaderMoreToolsCell = ({
	context,
}: IHeaderParams<TJournalRow, LessonGridContext>) => {
	const Button = context.moreToolsButton

	return (
		<Button
			className={MORE_TOOLS_CLASS}
			onClick={() => context.onHeaderMoreToolsClick?.()}
		/>
	)
}

// используется для рендера кнопки ⋮ в строке
export const RowMoreToolsCell = ({
	data,
	context,
}: ICellRendererParams<TJournalRow, unknown, LessonGridContext>) => {
	if (!data) {
		return null
	}

	const Button = context.moreToolsButton

	return (
		<Button
			className={MORE_TOOLS_CLASS}
			row={data}
			onClick={() => context.onRowMoreToolsClick?.(data)}
		/>
	)
}
