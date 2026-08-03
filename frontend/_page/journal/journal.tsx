"use client"

import type { TDiscipline } from "@/shared/model/discipline"
import {
	mockDisciplines,
	mockJournalRows,
	mockLessons,
} from "@/shared/model/mocks"
import { DisciplineCardTable } from "@/widgets/discipline-card-table"
import { LessonJournalTable } from "@/widgets/lesson-journal-table"
import { useCallback, useMemo, useState, type AnimationEvent } from "react"
import "./journal.css"

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
						(lesson) =>
							lesson.disciplineUuid === selectedDiscipline.uuid,
					)
				: [],
		[selectedDiscipline],
	)

	const [prevVisiblePanel, setPrevVisiblePanel] =
		useState<TVisiblePanel | null>(null)
	const [visiblePanel, setVisiblePanel] =
		useState<TVisiblePanel>("discipline")

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
				<DisciplineCardTable
					disciplines={mockDisciplines}
					onDisciplineClick={(discipline: TDiscipline) => {
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
