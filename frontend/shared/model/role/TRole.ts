import { TRoleRight } from "./TRoleRight"
import { Uuid } from "../utility-types/uuid"
import { IBaseEntity } from "../utility-types/base-entity"

export type TRole = {
	uuid: Uuid
	name: string
	rights: TRoleRight[]
	roleTypes: TRoleType[]
	isBase: boolean
} & IBaseEntity

export type TRoleTypeName = "СТУДЕНТ" | "ПРЕПОДАВАТЕЛЬ"

export type TRoleType = {
	uuid: Uuid
	name: TRoleTypeName
}
