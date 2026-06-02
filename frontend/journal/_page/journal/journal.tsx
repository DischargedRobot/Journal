"use client"

import { TDiscipline } from "@/shared/model/discipline"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import { DisciplineTable } from "@/widgets/discipline-table"
import { LessonJournalTable } from "@/widgets/lesson-journal-table"
import { useCallback, useMemo, useState, type AnimationEvent } from "react"
import "./journal.css"
import { TStudent } from "@/shared/model/student"
import { TGroup } from "@/shared/model/group"

const mockGroups: TGroup[] = [
	{
		uuid: "1",
		name: "1",
		code: "1",
		trainingDirectionUuid: "1",
		facultyUuid: "1",
		curatorsUuids: [],
		version: 1,
		admissionDate: "2024-12-25",
	},
	{
		uuid: "2",
		name: "2",
		code: "2",
		trainingDirectionUuid: "2",
		facultyUuid: "2",
		curatorsUuids: [],
		version: 1,
		admissionDate: "2024-12-25",
	},
]

const mockStudents: TStudent[] = [
	{
		uuid: "1",
		studentCode: 1,
		firstName: "Иван",
		lastName: "Иванов",
		patronymic: "Иванович",
		groupUuid: "1",
		group: mockGroups[0],
		brigadesUuids: [],
		brigades: [],
		lessons: new Map(),
		roles: [],
		version: 1,
	},
	{
		uuid: "2",
		studentCode: 2,
		firstName: "Петр",
		lastName: "Петров",
		patronymic: "Петрович",
		groupUuid: "2",
		group: mockGroups[1],
		brigadesUuids: [],
		brigades: [],
		lessons: new Map(),
		roles: [],
		version: 1,
	},
	{
		uuid: "3",
		studentCode: 3,
		firstName: "Сидор",
		lastName: "Сидоров",
		patronymic: "Сидорович",
		groupUuid: "3",
		group: mockGroups[2],
		brigadesUuids: [],
		brigades: [],
		lessons: new Map(),
		roles: [],
		version: 1,
	},
]

const disciplines: TDiscipline[] = [
	{
		uuid: "1",
		name: "Математика",
		shortName: "Мат.",
		type: "Лекция",
		isArchived: false,
		professorUuid: "1",
		groupUuid: "1",
		DisciplinesSet: "1",
	},
	{
		uuid: "2",
		name: "Физика",
		shortName: "Физ.",
		type: "Упражнение",
		isArchived: false,
		professorUuid: "2",
		groupUuid: "1",
		DisciplinesSet: "1",
	},
	{
		uuid: "3",
		name: "История",
		shortName: "Ист.",
		type: "Семинар",
		isArchived: false,
		professorUuid: "3",
		groupUuid: "2",
		DisciplinesSet: "1",
	},
	{
		uuid: "4",
		name: "Программирование",
		shortName: "Прог.",
		type: "Лабораторная",
		isArchived: false,
		professorUuid: "4",
		groupUuid: "2",
		DisciplinesSet: "2",
	},
	{
		uuid: "7",
		name: "Программирование",
		shortName: "Прог.",
		type: "Упражнение",
		isArchived: false,
		professorUuid: "4",
		groupUuid: "2",
		DisciplinesSet: "2",
	},
	{
		uuid: "5",
		name: "Английский язык",
		shortName: "АЯ",
		type: "Лекция",
		isArchived: true,
		professorUuid: "5",
		groupUuid: "3",
		DisciplinesSet: "2",
	},
]

const lessonTopic = "Преобразование Фурье"

const mockLessons: TLesson[] = Array.from({ length: 5 }, (_, index) => ({
	uuid: `lesson-${index + 1}`,
	code: 13,
	startDate: "2024-12-25T10:00:00Z",
	name: lessonTopic,
	shortName: "Преобр. Фурье",
	lessonTypeUuid: "lesson-type-1",
	disciplineUuid: "1",
	version: 0,
}))

const mockJournalRows: TJournalRow[] = [
	{
		student: mockStudents[0],
		order: 1,
		fullName: "Фамилия И. О.",
		lessons: new Map([
			[mockLessons[0].uuid, { presenceStatus: "Н", mark: "неуд." }],
			[mockLessons[1].uuid, { presenceStatus: "Н", mark: "зачтено" }],
			[mockLessons[2].uuid, { presenceStatus: "Н", mark: "зачтено" }],
			[mockLessons[3].uuid, { presenceStatus: "Н", mark: "зачтено" }],
		]),
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		student: mockStudents[1],
		order: 5,
		fullName: "Фамилия И. О.",
		lessons: new Map([
			[mockLessons[0].uuid, { presenceStatus: "1/2", mark: "зачтено" }],
			[mockLessons[1].uuid, { presenceStatus: "1/2", mark: "зачтено" }],
			[mockLessons[2].uuid, { presenceStatus: "1/2", mark: "зачтено" }],
			[mockLessons[3].uuid, { presenceStatus: "1/2", mark: "зачтено" }],
		]),
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		student: mockStudents[2],
		order: 3,
		fullName: "Фамилия И. О.",
		lessons: new Map([
			[mockLessons[0].uuid, { presenceStatus: "Б", mark: "5" }],
			[mockLessons[1].uuid, { presenceStatus: "Б", mark: "5" }],
			[mockLessons[2].uuid, { presenceStatus: "Б", mark: "5" }],
			[mockLessons[3].uuid, { presenceStatus: "Б", mark: "5" }],
		]),
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
]

type TVisiblePanel = "lesson" | "discipline"

const EXIT_ANIMATIONS = new Set(["fromDiscipline", "fromLesson"])

const getExitAnimationClass = (panel: TVisiblePanel) =>
	panel === "discipline" ? "fromDiscipline" : "fromLesson"

const getEnterAnimationClass = (panel: TVisiblePanel) =>
	panel === "discipline" ? "toDiscipline" : "toLesson"

const getPanelClassName = (
	panel: TVisiblePanel,
	visiblePanel: TVisiblePanel,
	prevVisiblePanel: TVisiblePanel | null,
): string => {
	const isAnimating = prevVisiblePanel != null
	const isExiting = isAnimating && prevVisiblePanel === panel
	const isEntering = isAnimating && visiblePanel === panel

	if (isExiting) {
		return `journal-panel__exit ${getExitAnimationClass(panel)}`
	}
	if (isEntering) {
		return `journal-panel__enter ${getEnterAnimationClass(panel)}`
	}
	if (visiblePanel === panel) {
		return "journal-panel__active"
	}
	return "journal-panel__inactive"
}

const Journal = () => {
	const [selectedDiscipline, setSelectedDiscipline] =
		useState<TDiscipline | null>(null)

	const selectedLessons = useMemo(
		() =>
			selectedDiscipline
				? mockLessons.filter(
					(lesson) => lesson.disciplineUuid === selectedDiscipline.uuid,
				)
				: [],
		[selectedDiscipline],
	)

	const [prevVisiblePanel, setPrevVisiblePanel] = useState<TVisiblePanel | null>(
		null,
	)
	const [visiblePanel, setVisiblePanel] = useState<TVisiblePanel>("discipline")

	const handleSwitchPanel = useCallback(
		(panel: TVisiblePanel) => {
			if (panel === visiblePanel) {
				return
			}
			setPrevVisiblePanel(visiblePanel)
			setVisiblePanel(panel)
		},
		[visiblePanel],
	)

	const handleAnimationEnd = useCallback(
		(event: AnimationEvent<HTMLDivElement>) => {
			if (event.target !== event.currentTarget) {
				return
			}
			if (!EXIT_ANIMATIONS.has(event.animationName)) {
				return
			}

			if (event.animationName === "fromLesson") {
				setSelectedDiscipline(null)
			}
			setPrevVisiblePanel(null)
		},
		[],
	)

	return (
		<div className="p-4 w-full justify-center overflow-x-auto journal-screen">
			<div
				className={`journal-panel ${getPanelClassName(
					"discipline",
					visiblePanel,
					prevVisiblePanel,
				)}`}
				onAnimationEnd={handleAnimationEnd}
			>
				<DisciplineTable
					disciplines={disciplines}
					onDisciplineClick={(discipline) => {
						setSelectedDiscipline(discipline)
						handleSwitchPanel("lesson")
					}}
				/>
			</div>
			<div
				className={`journal-panel ${getPanelClassName("lesson", visiblePanel, prevVisiblePanel)}`}
				onAnimationEnd={handleAnimationEnd}
			>
				<LessonJournalTable
					lessons={selectedLessons}
					discipline={selectedDiscipline ?? undefined}
					rows={mockJournalRows}
					title={selectedDiscipline?.name}
					onBackClick={() => handleSwitchPanel("discipline")}
				/>
			</div>
		</div>
	)
}

export default Journal
