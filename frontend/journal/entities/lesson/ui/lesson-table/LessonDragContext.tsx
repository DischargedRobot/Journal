"use client"

import { createContext, useContext } from "react"

/** uuid перетаскиваемой колонки; используется для затемнения заголовка и ячеек тела */
export const LessonDragContext = createContext<string | null>(null)

export const useIsLessonDragging = (lessonUuid: string) => {
    const activeLessonUuid = useContext(LessonDragContext)
    return activeLessonUuid === lessonUuid
}
