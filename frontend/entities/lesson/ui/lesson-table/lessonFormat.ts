import type { TLesson } from "@/shared/model/lesson"

export const formatLessonDate = (iso: string) => {
    const date = new Date(iso)
    const day = String(date.getDate()).padStart(2, "0")
    const month = String(date.getMonth() + 1).padStart(2, "0")
    return `${day}.${month}`
}

export const getLessonTopic = (lesson: TLesson) =>
    lesson.name ?? lesson.shortName ?? `Занятие ${lesson.code}`
