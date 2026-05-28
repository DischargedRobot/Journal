import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import type { useSortable } from "@dnd-kit/sortable"

export type LessonTableProps = {
    lessons: TLesson[]
    rows: TJournalRow[]
    selectedRowUuid?: string
    onRowSelect?: (uuid: string) => void
    onRowMenuClick?: (row: TJournalRow) => void
    onHeaderMenuClick?: () => void
}

export type LessonSortableHandle = {
    attributes: ReturnType<typeof useSortable>["attributes"]
    listeners: ReturnType<typeof useSortable>["listeners"]
    setNodeRef: ReturnType<typeof useSortable>["setNodeRef"]
    isDragging: boolean
}

export type LessonSortableHandlesSetter = {
    register: (lessonUuid: string, handle: LessonSortableHandle) => void
    unregister: (lessonUuid: string) => void
}

export type LessonHeaderCellProps = {
    lesson: TLesson
}

export type LessonBodyCellsProps = {
    lesson: TLesson
    row: TJournalRow
}

export type LessonColumnOverlayProps = {
    lesson: TLesson
    rows: TJournalRow[]
    colWidth: number
}
