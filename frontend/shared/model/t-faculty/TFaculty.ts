import { IBaseEntity } from "../utility-types/base-entity"
import { Uuid } from "../utility-types/uuid"

type TFaculty = {
	uuid: Uuid
	name: string
	shortName: string
} & IBaseEntity

export type { TFaculty }
