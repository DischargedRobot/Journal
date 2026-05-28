"use client"

import type { TLesson } from "@/shared/model/lesson"
import { arrayMove } from "@dnd-kit/sortable"
import { useCallback, useMemo, useState } from "react"

export const useOrderedLessons = (lessons: TLesson[]) => {
    /** Порядок колонок занятий (uuid); источник правды для orderedLessons */
    const [lessonOrder, setLessonOrder] = useState<string[]>([])

    // Сначала uuid из lessonOrder, затем новые уроки, которых ещё нет в порядке
    const orderedLessons = useMemo(() => {
        const lessonsByUuid = new Map(lessons.map((lesson) => [lesson.uuid, lesson]))
        const lessonIds = lessons.map((lesson) => lesson.uuid)
        const orderedLessonIds = [
            ...lessonOrder.filter((uuid) => lessonsByUuid.has(uuid)),
            ...lessonIds.filter((uuid) => !lessonOrder.includes(uuid)),
        ]

        return orderedLessonIds
            .map((uuid) => lessonsByUuid.get(uuid))
            .filter((lesson): lesson is TLesson => Boolean(lesson))
    }, [lessonOrder, lessons])

    /** Меняет lessonOrder через arrayMove; вызывается на dragOver и dragEnd */
    const moveLesson = useCallback(
        (activeId: string, overId?: string) => {
            if (!overId || activeId === overId) {
                return
            }

            setLessonOrder((currentOrder) => {
                const lessonIds = lessons.map((lesson) => lesson.uuid)
                const syncedOrder = [
                    ...currentOrder.filter((uuid) => lessonIds.includes(uuid)),
                    ...lessonIds.filter((uuid) => !currentOrder.includes(uuid)),
                ]
                const oldIndex = syncedOrder.indexOf(activeId)
                const newIndex = syncedOrder.indexOf(overId)

                if (oldIndex === -1 || newIndex === -1) {
                    return currentOrder
                }

                return arrayMove(syncedOrder, oldIndex, newIndex)
            })
        },
        [lessons],
    )

    return { orderedLessons, moveLesson }
}
