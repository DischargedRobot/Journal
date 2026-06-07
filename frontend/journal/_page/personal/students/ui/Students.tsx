"use client"

import { mockPersonalGroups, mockPersonalStudents } from "@/shared/model/mocks"
import Sidebar from "@/shared/ui/sidebar/Sidebar"
import PersonalStudentTable from "@/widgets/personla-student-table/ui/PersonalStudentTable"

const Students = () => {
	return (
		<>
			<Sidebar
				open={true}
				onClose={() => {}}
				title="Группы"
				sx={{
					height: "100%",
					flex: 1,
					flexShrink: 0,
				}}
				items={mockPersonalGroups.map((group) => ({
					text: group.name,
					key: `/personal/groups/${group.uuid}`,
					onClick: () => {},
				}))}
			/>
			<PersonalStudentTable students={mockPersonalStudents} />
		</>
	)
}

export default Students
