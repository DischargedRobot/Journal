import { TDepartment } from "../t-department/TDepartment"
import { mockFaculties } from "./mockFaculties"

const fitFaculty = mockFaculties[0]

export const mockDepartments: TDepartment[] = [
	{
		uuid: "1",
		name: "Кафедра программной инженерии",
		shortName: "ПИ",
		code: "ПИ",
		facultyUuid: fitFaculty.uuid,
		faculty: fitFaculty,
	},
]
