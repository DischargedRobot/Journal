import { Uuid } from "../utility-types/uuid"

export type TBrigade = {
	uuid: Uuid
	name: string
	isTemplateForGroup: boolean
	groupUuid?: Uuid | null
	studentsUuids: Uuid[]
	disciplinesUuids?: Uuid[]
	version: number
}
