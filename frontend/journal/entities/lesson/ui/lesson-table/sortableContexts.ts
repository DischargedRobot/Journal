import { createContext } from "react"
import type { LessonSortableHandle, LessonSortableHandlesSetter } from "./types"

export const LessonSortableHandlesContext = createContext<Record<string, LessonSortableHandle>>(
    {},
)

export const LessonSortableHandlesSetterContext =
    createContext<LessonSortableHandlesSetter | null>(null)
