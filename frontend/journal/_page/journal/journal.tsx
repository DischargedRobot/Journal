"use client"

import { TDiscipline } from "@/shared/model/discipline"
import type { TJournalRow, TLesson } from "@/shared/model/lesson"
import { DisciplineTable } from "@/widgets/discipline-table"
import { LessonJournalTable } from "@/widgets/lesson-journal-table"
import { useCallback, useMemo, useState, type AnimationEvent } from "react"
import "./journal.css"

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

const mockLessons: TLesson[] = Array.from({ length: 4 }, (_, index) => ({
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
		uuid: "student-1",
		order: 1,
		fullName: "Фамилия И. О.",
		cells: {
			"lesson-1": { presence: "Н", grade: "вы" },
			"lesson-2": { presence: "Н", grade: "зачтено" },
			"lesson-3": { presence: "Н", grade: "зачтено" },
			"lesson-4": { presence: "Н", grade: "зачтено" },
		},
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		uuid: "student-2",
		order: 5,
		fullName: "Фамилия И. О.",
		cells: {
			"lesson-1": { presence: "1/2", grade: "зачтено" },
			"lesson-2": { presence: "1/2", grade: "зачтено" },
			"lesson-3": { presence: "Н", grade: "зачтено" },
			"lesson-4": { presence: "1/2", grade: "зачтено" },
		},
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		uuid: "student-3",
		order: 3,
		fullName: "Фамилия И. О.",
		cells: {
			"lesson-1": { presence: "Б", grade: "5" },
			"lesson-2": { presence: "Н", grade: "5" },
			"lesson-3": { presence: "Б", grade: "5" },
			"lesson-4": { presence: "Б", grade: "5" },
		},
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		uuid: "student-4",
		order: 2,
		fullName: "Фамилия И. О.",
		cells: {},
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		uuid: "student-5",
		order: 4,
		fullName: "Фамилия И. О.",
		cells: {},
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

const renderPanel = (
	key: TVisiblePanel,
	onSwitchPanel: (panel: TVisiblePanel) => void,
	renderDiscipline: (onClick: () => void) => React.ReactNode,
	renderLesson: (onClick: () => void) => React.ReactNode,
) => {
	switch (key) {
		case "discipline":
			return renderDiscipline(() => onSwitchPanel("lesson"))
		case "lesson":
			return renderLesson(() => onSwitchPanel("discipline"))
	}
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

	const handleExitAnimationEnd = useCallback(
		(event: AnimationEvent<HTMLDivElement>) => {
			if (event.target !== event.currentTarget) {
				return
			}
			if (!EXIT_ANIMATIONS.has(event.animationName)) {
				return
			}
			setPrevVisiblePanel(null)
		},
		[],
	)

	const LessonJournalComponent = useCallback(
		(onClick: () => void) => (
			<LessonJournalTable
				lessons={selectedLessons}
				rows={mockJournalRows}
				title={selectedDiscipline?.name}
				onBackClick={() => {
					setSelectedDiscipline(null)
					onClick()
				}}
			/>
		),
		[selectedLessons, selectedDiscipline],
	)

	const DisciplineTableComponent = useCallback(
		(onClick: () => void) => (
			<DisciplineTable
				disciplines={disciplines}
				onDisciplineClick={(discipline) => {
					setSelectedDiscipline(discipline)
					onClick()
				}}
			/>
		),
		[],
	)

	const isAnimation = prevVisiblePanel != null

	return (
		<div className="p-4 w-full justify-center overflow-x-auto journal-screen">
			{prevVisiblePanel && (
				<div
					className={`journal-panel journal-panel--exit ${getExitAnimationClass(prevVisiblePanel)}`}
					onAnimationEnd={handleExitAnimationEnd}
				>
					{renderPanel(
						prevVisiblePanel,
						handleSwitchPanel,
						DisciplineTableComponent,
						LessonJournalComponent,
					)}
				</div>
			)}
			<div
				className={`journal-panel ${isAnimation ? `journal-panel--enter ${getEnterAnimationClass(visiblePanel)}` : ""}`}
			>
				{renderPanel(
					visiblePanel,
					handleSwitchPanel,
					DisciplineTableComponent,
					LessonJournalComponent,
				)}
			</div>
		</div>
	)
}

export default Journal
