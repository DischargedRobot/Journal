"use client"

import TableCell from "@mui/material/TableCell"
import { useIsLessonDragging } from "./LessonDragContext"
import { bodyCellSx, draggingColumnSx } from "./styles"
import type { LessonBodyCellsProps } from "./types"

/** Ячейки тела колонки; затемняются по activeLessonUuid, порядок задаёт родитель */
export const LessonBodyCells = ({ lesson, row }: LessonBodyCellsProps) => {
    const isLessonDragging = useIsLessonDragging(lesson.uuid)
    const cell = row.cells[lesson.uuid] ?? { presence: "", grade: "" }

    return (
        <>
            <TableCell sx={{ ...bodyCellSx, ...(isLessonDragging && draggingColumnSx) }}>
                {cell.presence}
            </TableCell>
            <TableCell sx={{ ...bodyCellSx, ...(isLessonDragging && draggingColumnSx) }}>
                {cell.grade}
            </TableCell>
        </>
    )
}
