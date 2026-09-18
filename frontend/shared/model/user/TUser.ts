import { TDepartment } from "../department"
import { TGroup } from "../group"
import { TRole } from "../role"
import { IBaseEntityWithVersion } from "../utility-types/base-entity"

export type TUserBase = {
	email: string
	firstName: string
	lastName: string
	patronymic: string
	role: TRole
} & IBaseEntityWithVersion

type TStudentUser = TUserBase & {
	role: TRole<"СТУДЕНТ">
	groups: TGroup[]
}
type TTeacherUser = TUserBase & {
	role: TRole<"ПРЕПОДАВАТЕЛЬ">
	departments: TDepartment[]
}

export const isStudentUser = (user: TUser): user is TStudentUser =>
	user.role.roleTypes.some((roleType) => roleType.name === "СТУДЕНТ")
export const isTeacherUser = (user: TUser): user is TTeacherUser =>
	user.role.roleTypes.some((roleType) => roleType.name === "ПРЕПОДАВАТЕЛЬ")

export type TUser = TStudentUser | TTeacherUser
