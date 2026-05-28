"use client"

import TableCell from "@mui/material/TableCell"
import { formatLessonDate, getLessonTopic } from "./lessonFormat"
import { headerCellSx } from "./styles"
import type { LessonHeaderCellProps } from "./types"
import { useLessonHeaderDrag } from "./useLessonHeaderDrag"

export const LessonMetaHeaderCell = ({ lesson }: LessonHeaderCellProps) => {
    const { setNodeRef, dragHandleProps, headerSx } = useLessonHeaderDrag(lesson.uuid, true)

    return (
        <TableCell
            ref={setNodeRef}
            colSpan={2}
            sx={{ ...headerCellSx, ...headerSx }}
            {...dragHandleProps}
        >
            №{lesson.code} {formatLessonDate(lesson.startDate)}
        </TableCell>
    )
}

export const LessonTopicHeaderCell = ({ lesson }: LessonHeaderCellProps) => {
    const { dragHandleProps, headerSx } = useLessonHeaderDrag(lesson.uuid)

    return (
        <TableCell
            colSpan={2}
            sx={{
                ...headerCellSx,
                whiteSpace: "normal",
                maxWidth: 160,
                ...headerSx,
            }}
            {...dragHandleProps}
        >
            {getLessonTopic(lesson)}
        </TableCell>
    )
}

export const LessonSubHeaderCells = ({ lesson }: LessonHeaderCellProps) => {
    const { dragHandleProps, headerSx } = useLessonHeaderDrag(lesson.uuid)

    return (
        <>
            <TableCell sx={{ ...headerCellSx, ...headerSx }} {...dragHandleProps}>
                Б/Н
            </TableCell>
            <TableCell sx={{ ...headerCellSx, ...headerSx }} {...dragHandleProps}>
                Оценка
            </TableCell>
        </>
    )
}
