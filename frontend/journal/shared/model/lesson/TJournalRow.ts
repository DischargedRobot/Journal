import { Uuid } from "../utility-types/uuid"
import { TPresencesStatus } from "../presences-status"
import { TStudent } from "../student/TStudent"
import { TLesson } from "./TLesson"

/** Строка журнала (студент) */
export type TJournalRow = {
	student: TStudent
	/** Порядковый № в журнале (может не совпадать с индексом) */
	order: number
	fullName: string
	lessons: TStudent["lessons"]
	percent: string | number
	attendedTotal: string
	attestation: string
}
