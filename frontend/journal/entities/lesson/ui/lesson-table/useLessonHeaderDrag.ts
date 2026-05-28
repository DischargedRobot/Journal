"use client"

import { useContext } from "react"
import { useIsLessonDragging } from "./LessonDragContext"
import { LessonSortableHandlesContext } from "./sortableContexts"
import { draggingColumnSx } from "./styles"

/**
 * Раздаёт drag-handle по ячейкам заголовка одной колонки.
 * setActivatorRef=true только на первой строке (№/дата) — якорь для измерений dnd-kit.
 */
export const useLessonHeaderDrag = (lessonUuid: string, setActivatorRef = false) => {
    const handles = useContext(LessonSortableHandlesContext)
    const handle = handles[lessonUuid]
    const isLessonDragging = useIsLessonDragging(lessonUuid)

    const cursor = handle?.isDragging ? "grabbing" : "grab"

    return {
        setNodeRef: setActivatorRef ? handle?.setNodeRef : undefined,
        dragHandleProps: handle
            ? {
                  ...handle.attributes,
                  ...handle.listeners,
                  style: { touchAction: "none" as const },
              }
            : {},
        headerSx: {
            cursor,
            touchAction: "none",
            ...(isLessonDragging && draggingColumnSx),
        },
    }
}
