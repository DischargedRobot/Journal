"use client"

import {
	mockDepartments,
	mockFaculties,
	mockProfessors,
} from "@/shared/model/mocks"
import Sidebar from "@/shared/ui/sidebar/Sidebar"
import { DepartmentsList } from "@/widgets/departments-list"

const Structure = () => {
	return (
		<>
			<Sidebar
				open={true}
				onClose={() => {}}
				title="Факультеты"
				sx={{
					height: "100%",
					flex: 1,
					flexShrink: 0,
				}}
				items={mockFaculties.map((faculty) => ({
					text: faculty.shortName,
					key: `${faculty.uuid}`,
					onClick: () => {},
				}))}
			/>
			<div className="p-10 w-full">
				<DepartmentsList
					items={mockDepartments.map((department) => ({
						department,
						professors: mockProfessors,
					}))}
					title="Кафедры"
				/>
			</div>
		</>
	)
}

export default Structure
