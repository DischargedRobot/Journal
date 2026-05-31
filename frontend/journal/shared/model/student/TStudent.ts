import { Uuid } from "../utility-types/uuid"
import { TBrigade } from "../brigade/TBrigade"
import { TGroup } from "../group/TGroup"
import { IBaseEntityWithVersion } from "../utility-types/base-entity"
import { TRole } from "../role/TRole"

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
	roles: TRole[]
} & IBaseEntityWithVersion
