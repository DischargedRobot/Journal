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
					flex: 1,
				}}
				items={mockFaculties.map((faculty) => ({
					text: faculty.shortName,
					key: `${faculty.uuid}`,
					onClick: () => {},
				}))}
			/>
			<div className="p-10 flex-5">
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
