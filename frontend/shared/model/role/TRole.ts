import { TRoleRight } from "./TRoleRight"
import { Uuid } from "../utility-types/uuid"
import { IBaseEntity } from "../utility-types/base-entity"

export type TRole<Type extends TRoleTypeName = TRoleTypeName> = {
	uuid: Uuid
	name: string
	rights: TRoleRight[]
	roleTypes: TRoleType<Type>[]
	isBase: boolean
} & IBaseEntity

export type TRoleTypeName = "СТУДЕНТ" | "ПРЕПОДАВАТЕЛЬ"

export type TRoleType<Type extends TRoleTypeName> = {
	uuid: Uuid
	name: Type
}
