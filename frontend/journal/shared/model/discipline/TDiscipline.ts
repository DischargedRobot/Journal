import { IBaseEntity } from "../utility-types/base-entity"
import { Uuid } from "../utility-types/uuid"

export type DisciplineType =
	| "Лекция"
	| "Упражнение"
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
	| "Упр."
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
	shortName: string
	type: DisciplineType
	isArchived: boolean
	professorUuid: Uuid
	groupUuid: Uuid
	DisciplinesSet: Uuid
} & IBaseEntity
