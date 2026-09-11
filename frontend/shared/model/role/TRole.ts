import { TRoleRightName } from "./TRoleRight"
import { Uuid } from "../utility-types/uuid"
import { IBaseEntity } from "../utility-types/base-entity"

export type TRole = {
	uuid: Uuid
	name: string
	rights: TRoleRightName[]
	roleTypes: TRoleType[]
	isBase: boolean
} & IBaseEntity

export type TRoleType = "СТУДЕНТ" | "TEACHER"
