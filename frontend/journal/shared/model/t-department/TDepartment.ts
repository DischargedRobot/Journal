import { TFaculty } from "../t-faculty/TFaculty"
import { Uuid } from "../utility-types/uuid"

export type TDepartment = {
	uuid: Uuid
	name: string
	shortName: string
	code: string
	facultyUuid: Uuid
	faculty: TFaculty
}
