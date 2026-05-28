"use client"

import type { ICellRendererParams, IHeaderParams } from "ag-grid-community"
import type { MouseEvent, PointerEvent, ReactNode } from "react"
import type { TJournalRow } from "@/shared/model/lesson"
import type { LessonGridContext } from "./types"

const stopGridPointer = (event: MouseEvent | PointerEvent) => {
	event.stopPropagation()
}

/** Клики не доходят до строки / ячейки grid. */
const MoreToolsSlot = ({ children }: { children: ReactNode }) => (
	<div
		className="lesson-grid-more-tools"
		onClick={stopGridPointer}
		onMouseDown={stopGridPointer}
		onPointerDown={stopGridPointer}
	>
		{children}
	</div>
)

export const HeaderMoreToolsCell = ({
	context,
}: IHeaderParams<TJournalRow, LessonGridContext>) => {
	const Button = context.moreToolsButton

	return (
		<MoreToolsSlot>
			<Button onClick={() => context.onHeaderMoreToolsClick?.()} />
		</MoreToolsSlot>
	)
}

export const RowMoreToolsCell = ({
	data,
	context,
}: ICellRendererParams<TJournalRow, unknown, LessonGridContext>) => {
	if (!data) {
		return null
	}

	const Button = context.moreToolsButton

	return (
		<MoreToolsSlot>
			<Button row={data} onClick={() => context.onRowMoreToolsClick?.(data)} />
		</MoreToolsSlot>
	)
}
