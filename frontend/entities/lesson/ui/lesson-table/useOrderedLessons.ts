"use client"

import type { TLesson } from "@/shared/model/lesson"
import { useCallback, useMemo, useState } from "react"

const arrayMove = <T,>(list: T[], from: number, to: number): T[] => {
	const next = [...list]
	const [item] = next.splice(from, 1)
	next.splice(to, 0, item)
	return next
}

export const useOrderedLessons = (lessons: TLesson[]) => {
	const [lessonOrder, setLessonOrder] = useState<string[]>([])

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

	const syncLessonOrder = useCallback((uuids: string[]) => {
		setLessonOrder(uuids)
	}, [])

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

	return { orderedLessons, moveLesson, syncLessonOrder }
}
