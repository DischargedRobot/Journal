import { TPerm } from "./TPerm"
import { Uuid } from "../utility-types/uuid"
import { IBaseEntity } from "../utility-types/base-entity"

export type TRole = {
	uuid: Uuid
	name: string
	permissions: TPerm[]
} & IBaseEntity
