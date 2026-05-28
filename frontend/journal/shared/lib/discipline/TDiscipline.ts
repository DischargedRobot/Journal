import { Uuid } from "../utility-types/uuid"

export type DisciplineType =
	| "Лекция"
	| "Практика"
	| "Лабораторная"
	| "Семинар"
	| "Курсовой проект"
	| "Колоквиум"
	| "Доп. курсы"
	| "НИИР"
	// | "Военная кафедра"
	| "Другое"

export type DisciplineTypeShort =
	| "Лек."
	| "Прак."
	| "Лаб."
	| "Сем."
	| "Курс."
	| "Колок."
	| "Доп. курсы"
	| "НИИР"
	// | "военка"
	| "Другое"

export type TDiscipline = {
	uuid: Uuid
	name: string
	type: DisciplineType
	isArchived: boolean
	professorUuid: Uuid
	groupUuid: Uuid
}
