import { IBaseEntityWithVersion } from "../utility-types/base-entity"
import { Uuid } from "../utility-types/uuid"

export type TGroup = {
	uuid: Uuid
	admissionDate: string // DateOnly сериализуется как строка (YYYY-MM-DD)
	code: string
	trainingDirectionUuid: Uuid
	facultyUuid: Uuid
	curatorsUuids: Uuid[]
} & IBaseEntityWithVersion
