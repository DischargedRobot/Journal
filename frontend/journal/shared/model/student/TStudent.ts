import { Uuid } from "../utility-types/uuid"
import { TBrigade } from "../brigade/TBrigade"
import { TGroup } from "../group/TGroup"
import { IBaseEntityWithVersion } from "../utility-types/base-entity"
import { TRole } from "../role/TRole"
import { TLesson } from "../lesson/TLesson"
import { TPresencesStatus } from "../presences-status"

export type TStudent = {
	uuid: Uuid
	studentCode: number
	firstName: string
	lastName: string
	patronymic?: string | null
	groupUuid: Uuid
	group: TGroup
	brigadesUuids: Uuid[]
	brigades: TBrigade[]
	lessons: Map<
		TLesson["uuid"],
		{ presenceStatus: TPresencesStatus; mark: string }
	>
	roles: TRole[]
} & IBaseEntityWithVersion
