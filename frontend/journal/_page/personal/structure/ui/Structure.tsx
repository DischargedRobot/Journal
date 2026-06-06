"use client"

import { mockDepartments, mockProfessors } from "@/shared/model/mocks"
import { DepartmentsList } from "@/widgets/departments-list"

const Structure = () => {
	return (
		<div className="p-10 w-full">
			<DepartmentsList
				items={mockDepartments.map((department) => ({
					department,
					professors: mockProfessors,
				}))}
				title="Кафедры"
			/>
		</div>
	)
}

export default Structure
