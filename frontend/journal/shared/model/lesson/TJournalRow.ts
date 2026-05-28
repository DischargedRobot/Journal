import { Uuid } from "../utility-types/uuid"

/** Ячейка занятия: посещаемость и оценка */
export type TJournalLessonCell = {
	presence: string
	grade: string
}

/** Строка журнала (студент) */
export type TJournalRow = {
	uuid: Uuid
	/** Порядковый № в журнале (может не совпадать с индексом) */
	order: number
	fullName: string
	cells: Record<Uuid, TJournalLessonCell>
	percent: string | number
	attendedTotal: string
	attestation: string
}
