import { TLesson } from "@/shared/model/lesson"

const lessonTopic = "Преобразование Фурье"

export const mockLessons: TLesson[] = Array.from({ length: 5 }, (_, index) => ({
	uuid: `lesson-${index + 1}`,
	code: 13,
	startDate: "2024-12-25T10:00:00Z",
	name: lessonTopic,
	shortName: "Преобр. Фурье",
	lessonTypeUuid: "lesson-type-1",
	disciplineUuid: "1",
	version: 0,
}))
