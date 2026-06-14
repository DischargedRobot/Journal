import { IBaseEntity } from "../utility-types/base-entity"
import { Uuid } from "../utility-types/uuid"
import { TGroup } from "../group"
import { TProfessor } from "../professor"

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
