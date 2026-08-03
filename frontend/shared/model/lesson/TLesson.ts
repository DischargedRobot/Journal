import { IBaseEntity } from "../utility-types/base-entity"
import { Uuid } from "../utility-types/uuid"
import { TDate } from "../date"

export type TLesson = {
	uuid: Uuid
	code: number
	startDate: TDate
	name?: string | null
	shortName?: string | null
	lessonTypeUuid: Uuid
	disciplineUuid: Uuid
} & IBaseEntity
