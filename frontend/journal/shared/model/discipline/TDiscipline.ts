import { IBaseEntity } from "../utility-types/base-entity"
import { Uuid } from "../utility-types/uuid"
import { TGroup } from "../group"
import { TProfessor } from "../professor"

export type DISCIPLINE_TYPE_SHORT_MAP = {
	[K in keyof typeof DISCIPLINE_TYPE_SHORT_MAP]: (typeof DISCIPLINE_TYPE_SHORT_MAP)[K]
}

export const DISCIPLINE_TYPE_SHORT_MAP = {
	Лекция: "Лек.",
	Упражнение: "Упр.",
	Лабораторная: "Лаб.",
	Семинар: "Сем.",
	"Курсовой проект": "Курс.",
	Колоквиум: "Колок.",
	"Доп. курсы": "Доп. курсы",
	НИИР: "НИИР",
	Другое: "Другое",
} as const

export type DISCIPLINE_TYPE_FULL_MAP = {
	[K in DISCIPLINE_TYPE_SHORT_MAP[keyof DISCIPLINE_TYPE_SHORT_MAP]]: KeyByValue<
		DISCIPLINE_TYPE_SHORT_MAP,
		K
	>
}

type KeyByValue<T, U extends T[keyof T]> = {
	[K in keyof T]: T[K] extends U ? K : never
}[keyof T]

export const DISCIPLINE_TYPE_FULL_MAP = Object.fromEntries(
	Object.entries(DISCIPLINE_TYPE_SHORT_MAP).map(([key, value]) => [
		value,
		key,
	]),
)

export type DisciplineType = keyof typeof DISCIPLINE_TYPE_SHORT_MAP
export type DisciplineTypeShort =
	(typeof DISCIPLINE_TYPE_SHORT_MAP)[DisciplineType]

export type TDiscipline = {
	uuid: Uuid
	name: string
	shortName: string
	type: DisciplineType
	isArchived: boolean
	professors: TProfessor[]
	groups: TGroup[]
	disciplinesSetUuid: Uuid
} & IBaseEntity
