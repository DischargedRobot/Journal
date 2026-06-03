"use client"

import { mockPersonalGroups, mockPersonalStudents } from "@/shared/model/mocks"
import Sidebar from "@/shared/ui/sidebar/Sidebar"
import PersonalStudentTable from "@/widgets/personla-student-table/ui/PersonalStudentTable"

const Students = () => {
	return (
		<div className="flex h-full w-full">
			<Sidebar open={true} onClose={() => { }} title="Группы" items={mockPersonalGroups.map((group, index) => ({
				text: group.name,
				href: `/personal/groups/${group.uuid}`,
				onClick: () => { },
				selected: index === 0,
			}))} />

			<div className="px-16 py-8">

				<PersonalStudentTable students={mockPersonalStudents} />
			</div>
		</div>
	)
}

export default Students
