import { TRole } from "@/shared/model/role/TRole"
import { TGroup } from "@/shared/model/group/TGroup"
import { TDepartment } from "@/shared/model/t-department"

export interface FormValues extends PERSONAL_FORM_FIELDS, LOGIN_FORM_FIELDS {}

// TODO: доделать сразу доавбление ролей
interface ISelectedRole {
	role: TRole | null
	department: TGroup | TDepartment | null
}

export interface PERSONAL_FORM_FIELDS {
	firstName: string
	lastName: string
	patronymic: string
	email: string
	personRole: "СТУДЕНТ" | "ПРЕПОДАВАТЕЛЬ"
	department: string | null
	group: string | null
}

export const REQUIRED_FIELDS: (keyof FormValues)[] = [
	"firstName",
	"lastName",
	"email",
	"personRole",
	"login",
	"password",
	"passwordConfirm",
]

export const PERSONAL_FIELDS: (keyof PERSONAL_FORM_FIELDS)[] = [
	"firstName",
	"lastName",
	"patronymic",
	"personRole",
	"group",
	"department",
	"email",
]

export interface LOGIN_FORM_FIELDS {
	login: string
	password: string
	passwordConfirm: string
}

export const LOGIN_FIELDS: (keyof LOGIN_FORM_FIELDS)[] = [
	"login",
	"password",
	"passwordConfirm",
]
