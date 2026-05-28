"use client"

import type { TLesson } from "@/shared/model/lesson"
import { useSortable } from "@dnd-kit/sortable"
import {
    useCallback,
    useContext,
    useLayoutEffect,
    useMemo,
    useState,
    type ReactNode,
} from "react"
import {
    LessonSortableHandlesContext,
    LessonSortableHandlesSetterContext,
} from "./sortableContexts"
import type { LessonSortableHandle } from "./types"

/**
 * Один useSortable на колонку (id = lessonUuid), но заголовок — три строки TableCell.
 * Registrar ничего не рендерит, а отдаёт listeners/ref в контекст для ячеек заголовка.
 */
const LessonSortableRegistrar = ({ lessonUuid }: { lessonUuid: string }) => {
    const setter = useContext(LessonSortableHandlesSetterContext)
    const sortable = useSortable({ id: lessonUuid })

    useLayoutEffect(() => {
        if (!setter) {
            return
        }

        setter.register(lessonUuid, {
            attributes: sortable.attributes,
            listeners: sortable.listeners,
            setNodeRef: sortable.setNodeRef,
            isDragging: sortable.isDragging,
        })

        return () => setter.unregister(lessonUuid)
    }, [lessonUuid, setter, sortable])

    return null
}

/** Собирает handles всех колонок и оборачивает TableHead (должен быть внутри SortableContext). */
export const LessonSortableHandlesProvider = ({
    lessons,
    children,
}: {
    lessons: TLesson[]
    children: ReactNode
}) => {
    const [handles, setHandles] = useState<Record<string, LessonSortableHandle>>({})

    const register = useCallback((lessonUuid: string, handle: LessonSortableHandle) => {
        setHandles((current) => ({ ...current, [lessonUuid]: handle }))
    }, [])

    const unregister = useCallback((lessonUuid: string) => {
        setHandles((current) => {
            if (!current[lessonUuid]) {
                return current
            }

            const next = { ...current }
            delete next[lessonUuid]
            return next
        })
    }, [])

    const setterValue = useMemo(
        () => ({ register, unregister }),
        [register, unregister],
    )

    return (
        <LessonSortableHandlesSetterContext.Provider value={setterValue}>
            <LessonSortableHandlesContext.Provider value={handles}>
                {lessons.map((lesson) => (
                    <LessonSortableRegistrar key={lesson.uuid} lessonUuid={lesson.uuid} />
                ))}
                {children}
            </LessonSortableHandlesContext.Provider>
        </LessonSortableHandlesSetterContext.Provider>
    )
}
