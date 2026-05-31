"use client"

import { StudentTable } from "@/entities/student"
import { TGroup } from "@/shared/model/group"
import { TRole } from "@/shared/model/role"
import { TStudent } from "@/shared/model/student"

const mockRoles: TRole[] = [
	{
		uuid: "11111111-1111-1111-1111-111111111101",
		name: "Студент",
		permissions: [],
	}
]

const mockStudents: TStudent[] = [
	{
		uuid: "11111111-1111-1111-1111-111111111101",
		studentCode: 111111,
		firstName: "Кирилл",
		lastName: "Авдеев",
		patronymic: "Владимирович",
		groupUuid: "22222222-2222-2222-2222-222222222201",
		brigadesUuids: [],
		brigades: [],
		roles: mockRoles,
		version: 1,
		group: {
			uuid: "22222222-2222-2222-2222-222222222201",
			code: "2415",
			name: "2415",
			admissionDate: "2026-01-01",
			trainingDirectionUuid: "33333333-3333-3333-3333-333333333301",
			facultyUuid: "44444444-4444-4444-4444-444444444401",
			curatorsUuids: [],
			version: 1,
		} satisfies TGroup,
	},
	{
		uuid: "11111111-1111-1111-1111-111111111102",
		studentCode: 111111,
		firstName: "Кирилл",
		lastName: "Авдеев",
		patronymic: "Владимирович",
		groupUuid: "22222222-2222-2222-2222-222222222201",
		brigadesUuids: [],
		brigades: [],
		version: 1,
		group: {
			uuid: "22222222-2222-2222-2222-222222222201",
			code: "2415",
			name: "2415",
			admissionDate: "2026-01-01",
			trainingDirectionUuid: "33333333-3333-3333-3333-333333333301",
			facultyUuid: "44444444-4444-4444-4444-444444444401",
			curatorsUuids: [],
			version: 1,
		} satisfies TGroup,
		roles: mockRoles,
	},
	{
		uuid: "11111111-1111-1111-1111-111111111104",
		studentCode: 111111,
		firstName: "Кирилл",
		lastName: "Авдеев",
		patronymic: "Владимирович",
		groupUuid: "22222222-2222-2222-2222-222222222201",
		brigadesUuids: [],
		brigades: [],
		version: 1,
		group: {
			uuid: "22222222-2222-2222-2222-222222222201",
			code: "2415",
			name: "2415",
			admissionDate: "2026-01-01",
			trainingDirectionUuid: "33333333-3333-3333-3333-333333333301",
			facultyUuid: "44444444-4444-4444-4444-444444444401",
			curatorsUuids: [],
			version: 1,
		} satisfies TGroup,
		roles: mockRoles,
	} satisfies TStudent,
] satisfies TStudent[]

const Students = () => {
	return (
		<div className="p-6">
			<StudentTable students={mockStudents} />
		</div>
	)
}

export default Students
