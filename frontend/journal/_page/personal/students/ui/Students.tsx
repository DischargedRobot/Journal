"use client"

import { TGroup } from "@/shared/model/group"
import { TRole } from "@/shared/model/role"
import { TStudent } from "@/shared/model/student"
import Sidebar from "@/shared/ui/sidebar/Sidebar"
import { StatCard } from "@/shared/ui/stat-card"
import { VisitIcon } from "@/shared/ui/visit-icon"
import PersonalStudentTable from "@/widgets/personla-student-table/ui/PersonalStudentTable"
import Box from "@mui/material/Box"
import { AttestationIcon } from "@/shared/ui/attestation-icon"

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

const mockGroups: TGroup[] = [
	{
		uuid: "22222222-2222-2222-2222-222222222201",
		code: "2415",
		name: "2415",
		admissionDate: "2026-01-01",
		trainingDirectionUuid: "33333333-3333-3333-3333-333333333301",
		facultyUuid: "44444444-4444-4444-4444-444444444401",
		curatorsUuids: [],
		version: 1,
	},
	{
		uuid: "22222222-2222-2222-2222-222222222202",
		code: "2416",
		name: "2416",
		admissionDate: "2026-01-01",
		trainingDirectionUuid: "33333333-3333-3333-3333-333333333301",
		facultyUuid: "44444444-4444-4444-4444-444444444401",
		curatorsUuids: [],
		version: 1,
	},
	{
		uuid: "22222222-2222-2222-2222-222222222203",
		code: "2417",
		name: "2417",
		admissionDate: "2026-01-01",
		trainingDirectionUuid: "33333333-3333-3333-3333-333333333301",
		facultyUuid: "44444444-4444-4444-4444-444444444401",
		curatorsUuids: [],
		version: 1,
	},
]

const Students = () => {
	return (
		<div className="flex h-full">
			<Sidebar open={true} onClose={() => { }} title="Группы" items={mockGroups.map((group, index) => ({
				text: group.name,
				href: `/personal/groups/${group.uuid}`,
				onClick: () => { },
				selected: index === 0,
			}))} />

			<div className="flex flex-col gap-4 px-16 py-8">
				<Box className="flex gap-4">
					<StatCard icon={<VisitIcon />} value={"70%"} label="Посещаемость" />
					<StatCard icon={<AttestationIcon />} value={"4.5"} label="Средний балл" />
				</Box>
				<PersonalStudentTable students={mockStudents} />
			</div>
		</div>
	)
}

export default Students
