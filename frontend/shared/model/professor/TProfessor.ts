import { Uuid } from "../utility-types/uuid"

export type TProfessor = {
	uuid: Uuid
	departmentUuid: Uuid
	postUuid: Uuid
	postName: string
	academicYearUuid: Uuid
	firstName: string
	lastName: string
	patronymic?: string | null
	groupCuratorUuids: Uuid[]
	disciplinesUuids: Uuid[]
	version: number
}
