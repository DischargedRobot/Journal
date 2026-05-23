import { Uuid } from "../utility-types/uuid"

export type TDiscipline = {
	uuid: Uuid
	name: string
	isArchived: boolean
	professorUuid: Uuid
	groupUuid: Uuid
}
