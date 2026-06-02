import { TStudent } from "../student/TStudent"

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
