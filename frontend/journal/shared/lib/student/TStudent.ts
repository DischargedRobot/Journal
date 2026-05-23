import { Uuid } from "../utility-types/uuid"

export type TStudent = {
	uuid: Uuid
	studentCode: number
	firstName: string
	lastName: string
	patronymic?: string | null
	groupUuid: Uuid
	brigadesUuids: Uuid[]
	version: number
}
