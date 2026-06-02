"use client"

import { mockPersonalGroups, mockPersonalStudents } from "@/shared/model/mocks"
import Sidebar from "@/shared/ui/sidebar/Sidebar"
import { StatCard } from "@/shared/ui/stat-card"
import { VisitIcon } from "@/shared/ui/visit-icon"
import PersonalStudentTable from "@/widgets/personla-student-table/ui/PersonalStudentTable"
import Box from "@mui/material/Box"
import { AttestationIcon } from "@/shared/ui/attestation-icon"

const Students = () => {
	return (
		<div className="flex h-full">
			<Sidebar open={true} onClose={() => { }} title="Группы" items={mockPersonalGroups.map((group, index) => ({
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
				<PersonalStudentTable students={mockPersonalStudents} />
			</div>
		</div>
	)
}

export default Students
