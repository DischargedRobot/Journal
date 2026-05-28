import { IBaseEntity } from "../utility-types/base-entity"
import { Uuid } from "../utility-types/uuid"

export type TLesson = {
	uuid: Uuid
	code: number
	startDate: string
	name?: string | null
	shortName?: string | null
	lessonTypeUuid: Uuid
	disciplineUuid: Uuid
} & IBaseEntity
